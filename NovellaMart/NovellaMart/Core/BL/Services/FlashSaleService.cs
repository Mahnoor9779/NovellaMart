using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.BL.Data_Structures;

//FlashSaleBL Sale Service
namespace NovellaMart.Core.BL.Services
{
    public class FlashSaleService
    {
        private FlashSaleBL _activeSale;
        private Dictionary<int, CircularQueue<CustomerRequestBL>> _productQueues;
        private Dictionary<string, string> _userRequestStatus;
        private Dictionary<string, DateTime> _allocationExpiry;
        private HashSet<string> _inCheckoutProcess = new();

        // In-memory logs only (No File Handling)
        private List<string> _activityLogs;
        private readonly FlashSaleCrudService _crudService;
        private Dictionary<string, CustomerRequestBL> _activeRequests;

        public FlashSaleService(FlashSaleCrudService crudService)
        {
            _crudService = crudService;
            
            // Try loading state from file
            var loadedData = NovellaMart.Core.DL.FileHandler.LoadData<FlashSaleRuntimeData>("flash_sale_runtime.json");
            
            if (loadedData != null)
            {
                _productQueues = loadedData.ProductQueues ?? new Dictionary<int, CircularQueue<CustomerRequestBL>>();
                _userRequestStatus = loadedData.UserRequestStatus ?? new Dictionary<string, string>();
                _allocationExpiry = loadedData.AllocationExpiry ?? new Dictionary<string, DateTime>();
                _inCheckoutProcess = loadedData.InCheckoutProcess ?? new HashSet<string>();
                _activityLogs = loadedData.ActivityLogs ?? new List<string>();
                _activeRequests = loadedData.ActiveRequests ?? new Dictionary<string, CustomerRequestBL>();
            }
            else
            {
                _activityLogs = new List<string>();
                _productQueues = new Dictionary<int, CircularQueue<CustomerRequestBL>>();
                _userRequestStatus = new Dictionary<string, string>();
                _activeRequests = new Dictionary<string, CustomerRequestBL>();
                _allocationExpiry = new Dictionary<string, DateTime>();
            }
        }

        private void SaveRuntimeState()
        {
            var data = new FlashSaleRuntimeData 
            {
                ProductQueues = _productQueues,
                UserRequestStatus = _userRequestStatus,
                AllocationExpiry = _allocationExpiry,
                InCheckoutProcess = _inCheckoutProcess,
                ActivityLogs = _activityLogs,
                ActiveRequests = _activeRequests
            };
            NovellaMart.Core.DL.FileHandler.SaveData("flash_sale_runtime.json", data);
        }

        public void SetActiveSale(FlashSaleBL sale)
        {
            if (sale == null) return;

            // FIX: Only re-initialize if it's a NEW or DIFFERENT sale
            if (_activeSale != null && _activeSale.flash_sale_id == sale.flash_sale_id)
            {
                return;
            }

            _activeSale = sale;
            _productQueues = new Dictionary<int, CircularQueue<CustomerRequestBL>>();
            _userRequestStatus = new Dictionary<string, string>();

            if (_activeSale.fs_items == null) return;

            var node = _activeSale.fs_items.head;
            while (node != null)
            {
                var product = node.Data;
                if (product != null && !_productQueues.ContainsKey(product.product_id))
                {
                    _productQueues.Add(product.product_id, new CircularQueue<CustomerRequestBL>(50));
                }
                node = node.Next;
            }
        }

        public FlashSaleBL GetActiveSale() => _activeSale;

        public string JoinFlashSaleQueue(CustomerBL customer, int productId)
        {
            if (_activeSale.status != "ACTIVE") return "Sale Ended";

            string statusKey = $"{customer.user_id}_{productId}";

            if (_userRequestStatus.ContainsKey(statusKey))
                return _userRequestStatus[statusKey];

            if (!_productQueues.ContainsKey(productId))
                return "Product Not in Sale";

            var targetQueue = _productQueues[productId];

            ProductBL saleItem = null;
            var node = _activeSale.fs_items.head;
            while (node != null) { if (node.Data.product_id == productId) { saleItem = node.Data; break; } node = node.Next; }

            CustomerRequestBL request = new CustomerRequestBL();
            request.request_id = DateTime.Now.Ticks;
            request.customer = customer;
            request.product = saleItem;
            request.requestTime = DateTime.Now;
            request.allocated = false;

            if (targetQueue.IsFull()) return "Queue Full";

            targetQueue.Enqueue(request);
            _userRequestStatus.Add(statusKey, "Queued");
            _activeRequests.Add(statusKey, request);
            
            SaveRuntimeState(); // PERSISTENCE

            AddLog($"[{DateTime.Now.ToLongTimeString()}] User {customer.user_id} joined queue for {saleItem.name}");

            return "Queued";
        }

        // NEW: Call this method when the Sale Ends to process the priority heap
        public void FinalizeAllocations()
        {
            if (_activeSale == null) return;

            foreach (var productId in _productQueues.Keys)
            {
                var bufferQueue = _productQueues[productId];

                // 1. DRAIN THE QUEUE: Move people from the Circular Queue to the Status Dictionary
                while (!bufferQueue.IsEmpty())
                {
                    var request = bufferQueue.Dequeue();
                    if (request == null) continue;
                    string statusKey = $"{request.customer.user_id}_{productId}";

                    if (!_userRequestStatus.ContainsKey(statusKey) || _userRequestStatus[statusKey] == "Queued")
                    {
                        _userRequestStatus[statusKey] = "Rejected"; // Mark as Rejected so they are eligible for the Heap
                        _activeRequests[statusKey] = request;
                    }
                }

                // 2. FORCE RE-STATUS: Also convert any existing "Queued" entries to "Rejected"
                var queuedKeys = _userRequestStatus.Where(x => x.Value == "Queued" && x.Key.EndsWith($"_{productId}")).ToList();
                foreach (var key in queuedKeys)
                {
                    _userRequestStatus[key.Key] = "Rejected";
                }

                // 3. Run the assignment
                PerformStockAssignment(productId);
            }

            // 3. Global Sale Status Management
            if (DateTime.Now >= _activeSale.endTime)
            {
                _activeSale.status = "ENDED";
                _crudService.UpdateFlashSale(_activeSale);
            }

            SaveRuntimeState();
        }

        public void PerformStockAssignment(int productId)
        {
            // 1. Locate the product node in the custom Linked List to check live stock
            ProductBL liveProduct = null;
            var node = _activeSale.fs_items.head;
            while (node != null)
            {
                if (node.Data.product_id == productId) { liveProduct = node.Data; break; }
                node = node.Next;
            }

            // If no stock is available (none left, or none returned from expiry), stop.
            if (liveProduct == null || liveProduct.stock <= 0) return;

            // 2. REFILL HEAP: Prepare the priority queue for this specific assignment pass
            while (!_activeSale.allocation_heap.IsEmpty()) _activeSale.allocation_heap.Dequeue();

            // Find everyone currently "Rejected". 
            // CRITICAL: This excludes "Expired" users, meaning they never get re-added to the heap.
            var eligibleKeys = _userRequestStatus
                .Where(x => x.Key.EndsWith($"_{productId}") && x.Value == "Rejected")
                .ToList();

            foreach (var entry in eligibleKeys)
            {
                if (_activeRequests.TryGetValue(entry.Key, out var request))
                {
                    // Enqueue into Heap using join time Ticks (Fairness Engine)
                    _activeSale.allocation_heap.Enqueue(request, request.requestTime.Ticks);
                }
            }

            // 3. ALLOCATION LOOP: Assign stock until product is empty or heap is empty
            while (liveProduct.stock > 0 && !_activeSale.allocation_heap.IsEmpty())
            {
                var winner = _activeSale.allocation_heap.Dequeue();
                string key = $"{winner.customer.user_id}_{productId}";

                liveProduct.stock--;
                winner.allocated = true;
                _userRequestStatus[key] = "Allocated";
                _allocationExpiry[key] = DateTime.Now.AddMinutes(2); // Give them their checkout window

                AddLog($"[{DateTime.Now.ToShortTimeString()}] Product {productId} assigned to User {winner.customer.user_id}");
            }
        }

        // --- UNJOIN LOGIC ---
        public void LeaveFlashSaleQueue(int userId, int productId)
        {
            string statusKey = $"{userId}_{productId}";

            if (_userRequestStatus.ContainsKey(statusKey))
            {
                _userRequestStatus.Remove(statusKey);
                _activeRequests.Remove(statusKey);
                SaveRuntimeState(); // PERSISTENCE
                AddLog($"[{DateTime.Now.ToLongTimeString()}] User {userId} LEFT queue for Product {productId}");
            }
        }

        private void AddLog(string message)
        {
            _activityLogs.Insert(0, message);
            if (_activityLogs.Count > 50) _activityLogs.RemoveAt(_activityLogs.Count - 1);
            // File saving removed
        }

        public string GetUserStatus(int userId, int productId)
        {
            string key = $"{userId}_{productId}";
            if (_userRequestStatus.ContainsKey(key)) return _userRequestStatus[key];
            return "None";
        }

        public int GetQueuePosition(int userId, int productId)
        {
            if (_productQueues.ContainsKey(productId)) return _productQueues[productId].Size();
            return 0;
        }

        public int GetTotalQueueCount(int productId)
        {
            //int total = 0;
            //foreach (var queue in _productQueues.Values)
            //{
            //    total += queue.Size();
            //}
            //return total;
            return _userRequestStatus.Count(x => x.Value == "Queued");
        }

        public int GetQueueCount(int productId)
        {
            return _userRequestStatus.Count(x => x.Key.EndsWith($"_{productId}") && x.Value == "Queued");
        }

        public List<string> GetActivityLogs()
        {
            return _activityLogs;
        }

        public List<CustomerRequestBL> GetQueueSnapshot(int productId)
        {
            return _userRequestStatus
                .Where(x => x.Key.EndsWith($"_{productId}"))
                .Select(x => {
                    if (_activeRequests.TryGetValue(x.Key, out var storedRequest))
                    {
                        // CRITICAL: We must ensure the object's internal allocated 
                        // state matches the Dictionary's "Ordered" or "Allocated" state
                        var currentStatus = x.Value;
                        storedRequest.allocated = (currentStatus == "Allocated" || currentStatus == "Ordered");
                        return storedRequest;
                    }
                    return null;
                })
                .Where(r => r != null)
                .ToList();
        }

        public List<CustomerRequestBL> GetQueueSnapshotAllocation(int productId)
        {
            // Ensure this returns everyone, including winners
            return _userRequestStatus
                .Where(x => x.Key.EndsWith($"_{productId}"))
                .Select(x => {
                    if (_activeRequests.TryGetValue(x.Key, out var storedRequest))
                    {
                        // Sync the allocated boolean based on the dictionary status
                        storedRequest.allocated = (_userRequestStatus[x.Key] == "Allocated");
                        return storedRequest;
                    }
                    return null;
                })
                .Where(r => r != null)
                .ToList();
        }

        public void MarkAsInCheckout(int userId, int productId)
        {
            string key = $"{userId}_{productId}";
            _inCheckoutProcess.Add(key);
            SaveRuntimeState(); // PERSISTENCE
        }

        public bool IsAlreadyInCheckout(int userId, int productId)
        {
            return _inCheckoutProcess.Contains($"{userId}_{productId}");
        }

        public void ReleaseExpiredAllocations()
        {
            var now = DateTime.Now;
            var keysToRelease = _allocationExpiry
                .Where(x => now > x.Value && _userRequestStatus[x.Key] == "Allocated")
                .Select(x => x.Key).ToList();

            foreach (var key in keysToRelease)
            {
                // FIX: If the user already bought it, don't return the stock!
                if (_userRequestStatus.ContainsKey(key) && _userRequestStatus[key] == "Ordered")
                    continue;

                var parts = key.Split('_');
                int productId = int.Parse(parts[1]);

                // 1. Return stock
                var node = _activeSale?.fs_items?.head;
                while (node != null)
                {
                    if (node.Data.product_id == productId) { node.Data.stock++; break; }
                    node = node.Next;
                }

                // 2. IMPORTANT: Clear the checkout lock so the next winner isn't blocked
                _inCheckoutProcess.Remove(key);

                // 3. Mark as Expired
                _userRequestStatus[key] = "Expired";
            }
            if(keysToRelease.Count > 0) SaveRuntimeState(); // PERSISTENCE
        }

        public void MarkAsOrdered(int userId, int productId)
        {
            string key = $"{userId}_{productId}";
            if (_userRequestStatus.ContainsKey(key))
            {
                _userRequestStatus[key] = "Ordered";
                _inCheckoutProcess.Remove(key); // Unlock the checkout
                SaveRuntimeState();
            }
        }
    }
}