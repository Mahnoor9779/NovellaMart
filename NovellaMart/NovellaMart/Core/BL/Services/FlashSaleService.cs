using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.BL.Data_Structures;

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
            _activityLogs = new List<string>();
            // FIX: Initialize here so they are never null
            _productQueues = new Dictionary<int, CircularQueue<CustomerRequestBL>>();
            _userRequestStatus = new Dictionary<string, string>();
            _activeRequests = new Dictionary<string, CustomerRequestBL>();
            _allocationExpiry = new Dictionary<string, DateTime>();
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

            AddLog($"[{DateTime.Now.ToLongTimeString()}] User {customer.user_id} joined queue for {saleItem.name}");

            return "Queued";
        }

        // NEW: Call this method when the Sale Ends to process the priority heap
        public void FinalizeAllocations()
        {
            foreach (var productId in _productQueues.Keys)
            {
                var bufferQueue = _productQueues[productId];

                // Find the live product reference to update master stock
                ProductBL liveProduct = null;
                var node = _activeSale.fs_items.head;
                while (node != null)
                {
                    if (node.Data.product_id == productId) { liveProduct = node.Data; break; }
                    node = node.Next;
                }

                if (liveProduct == null) continue;

                // 1. Move from Circular Buffer to Heap (Priority Sorting)
                while (!bufferQueue.IsEmpty())
                {
                    var request = bufferQueue.Dequeue();
                    string statusKey = $"{request.customer.user_id}_{productId}";

                    if (_userRequestStatus.ContainsKey(statusKey))
                    {
                        // Priority logic: Earliest Join Time gets priority
                        // Heap is a Max-Heap, so we use MaxValue - Ticks
                        long priorityValue = request.requestTime.Ticks;
                        _activeSale.allocation_heap.Enqueue(request, priorityValue);
                    }
                }

                // 2. Allocation Loop
                while (liveProduct.stock > 0 && !_activeSale.allocation_heap.IsEmpty())
                {
                    var winner = _activeSale.allocation_heap.Dequeue();
                    string winnerKey = $"{winner.customer.user_id}_{productId}";

                    liveProduct.stock--;
                    winner.allocated = true;
                    _userRequestStatus[winnerKey] = "Allocated"; // Update status, don't remove

                    _allocationExpiry[winnerKey] = DateTime.Now.AddMinutes(2);
                }

                // 3. Cleanup Losers
                while (!_activeSale.allocation_heap.IsEmpty())
                {
                    var loser = _activeSale.allocation_heap.Dequeue();
                    string loserKey = $"{loser.customer.user_id}_{productId}";

                    // This ensures they stay in the dropdown but show as Rejected
                    _userRequestStatus[loserKey] = "Rejected";
                }

            }
            if (_activeSale != null)
            {
                _activeSale.status = "ENDED";
                _crudService.UpdateFlashSale(_activeSale); // This will now work
                AddLog($"[{DateTime.Now.ToShortTimeString()}] Sale Finalized and Persisted to File.");
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

        public int GetTotalQueueCount()
        {
            int total = 0;
            foreach (var queue in _productQueues.Values)
            {
                total += queue.Size();
            }
            return total;
        }

        public int GetQueueCount(int productId)
        {
            if (_productQueues.ContainsKey(productId))
                return _productQueues[productId].Size();
            return 0;
        }

        public List<string> GetActivityLogs()
        {
            return _activityLogs;
        }

        public List<CustomerRequestBL> GetQueueSnapshot(int productId)
        {
            // Return ALL users associated with this product ID regardless of status
            return _userRequestStatus
                .Where(x => x.Key.EndsWith($"_{productId}")) // Removed "&& x.Value == 'Queued'"
                .Select(x => {
                    if (_activeRequests.TryGetValue(x.Key, out var storedRequest))
                    {
                        // Ensure the status matches the current dictionary value
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
        }
    }
}