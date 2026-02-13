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

        private List<string> _activityLogs;
        private readonly FlashSaleCrudService _crudService;
        private Dictionary<string, CustomerRequestBL> _activeRequests;
        private int _loadedFlashSaleId = 0;

        public FlashSaleService(FlashSaleCrudService crudService)
        {
            _crudService = crudService;
            
            var loadedData = NovellaMart.Core.DL.FileHandler.LoadData<FlashSaleRuntimeData>("flash_sale_runtime.json");
            
            if (loadedData != null)
            {
                _productQueues = loadedData.ProductQueues ?? new Dictionary<int, CircularQueue<CustomerRequestBL>>();
                _userRequestStatus = loadedData.UserRequestStatus ?? new Dictionary<string, string>();
                _allocationExpiry = loadedData.AllocationExpiry ?? new Dictionary<string, DateTime>();
                _inCheckoutProcess = loadedData.InCheckoutProcess ?? new HashSet<string>();
                _activityLogs = loadedData.ActivityLogs ?? new List<string>();
                _activeRequests = loadedData.ActiveRequests ?? new Dictionary<string, CustomerRequestBL>();
                _loadedFlashSaleId = loadedData.ActiveFlashSaleId;
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
                ActiveRequests = _activeRequests,
                ActiveFlashSaleId = _activeSale?.flash_sale_id ?? 0
            };
            NovellaMart.Core.DL.FileHandler.SaveData("flash_sale_runtime.json", data);
        }

        public void SetActiveSale(FlashSaleBL sale)
        {
            if (sale == null) return;

            if (_activeSale != null && _activeSale.flash_sale_id == sale.flash_sale_id)
            {
                return;
            }

            bool isResuming = (_activeSale == null && sale.flash_sale_id == _loadedFlashSaleId);

            _activeSale = sale;

            if (!isResuming)
            {
                _productQueues = new Dictionary<int, CircularQueue<CustomerRequestBL>>();
                _userRequestStatus = new Dictionary<string, string>();
            }

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

        public void FinalizeAllocations()
        {
            if (_activeSale == null) return;

            foreach (var productId in _productQueues.Keys)
            {
                var bufferQueue = _productQueues[productId];

                while (!bufferQueue.IsEmpty())
                {
                    var request = bufferQueue.Dequeue();
                    if (request == null) continue;
                    string statusKey = $"{request.customer.user_id}_{productId}";

                    if (!_userRequestStatus.ContainsKey(statusKey) || _userRequestStatus[statusKey] == "Queued")
                    {
                        _userRequestStatus[statusKey] = "Rejected";
                        _activeRequests[statusKey] = request;
                    }
                }

                var queuedKeys = _userRequestStatus.Where(x => x.Value == "Queued" && x.Key.EndsWith($"_{productId}")).ToList();
                foreach (var key in queuedKeys)
                {
                    _userRequestStatus[key.Key] = "Rejected";
                }

                PerformStockAssignment(productId);
            }

            if (DateTime.Now >= _activeSale.endTime)
            {
                _activeSale.status = "ENDED";
                _crudService.UpdateFlashSale(_activeSale);
            }

            SaveRuntimeState();
        }

        public void PerformStockAssignment(int productId)
        {
            ProductBL liveProduct = null;
            var node = _activeSale.fs_items.head;
            while (node != null)
            {
                if (node.Data.product_id == productId) { liveProduct = node.Data; break; }
                node = node.Next;
            }

            if (liveProduct == null || liveProduct.stock <= 0) return;

            while (!_activeSale.allocation_heap.IsEmpty()) _activeSale.allocation_heap.Dequeue();

            var eligibleKeys = _userRequestStatus
                .Where(x => x.Key.EndsWith($"_{productId}") && x.Value == "Rejected")
                .ToList();

            foreach (var entry in eligibleKeys)
            {
                if (_activeRequests.TryGetValue(entry.Key, out var request))
                {
                    _activeSale.allocation_heap.Enqueue(request, request.requestTime.Ticks);
                }
            }

            while (liveProduct.stock > 0 && !_activeSale.allocation_heap.IsEmpty())
            {
                var winner = _activeSale.allocation_heap.Dequeue();
                string key = $"{winner.customer.user_id}_{productId}";

                liveProduct.stock--;
                winner.allocated = true;
                _userRequestStatus[key] = "Allocated";
                _allocationExpiry[key] = DateTime.Now.AddMinutes(2);

                AddLog($"[{DateTime.Now.ToShortTimeString()}] Product {productId} assigned to User {winner.customer.user_id}");
            }
        }

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
            return _userRequestStatus
                .Where(x => x.Key.EndsWith($"_{productId}"))
                .Select(x => {
                    if (_activeRequests.TryGetValue(x.Key, out var storedRequest))
                    {
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
            SaveRuntimeState();
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
                if (_userRequestStatus.ContainsKey(key) && _userRequestStatus[key] == "Ordered")
                    continue;

                var parts = key.Split('_');
                int productId = int.Parse(parts[1]);

                var node = _activeSale?.fs_items?.head;
                while (node != null)
                {
                    if (node.Data.product_id == productId) { node.Data.stock++; break; }
                    node = node.Next;
                }

                _inCheckoutProcess.Remove(key);

                _userRequestStatus[key] = "Expired";
            }
            if(keysToRelease.Count > 0) SaveRuntimeState();
        }

        public void MarkAsOrdered(int userId, int productId)
        {
            string key = $"{userId}_{productId}";
            if (_userRequestStatus.ContainsKey(key))
            {
                _userRequestStatus[key] = "Ordered";
                _inCheckoutProcess.Remove(key);
                SaveRuntimeState();
            }
        }
    }
}