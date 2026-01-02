using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Services
{
    public class FlashSaleService
    {
        private FlashSaleBL _activeSale;
        private Dictionary<int, CircularQueue<CustomerRequestBL>> _productQueues;
        private Dictionary<string, string> _userRequestStatus;

        // In-memory logs only (No File Handling)
        private List<string> _activityLogs;

        public FlashSaleService()
        {
            _activityLogs = new List<string>();
        }

        public void SetActiveSale(FlashSaleBL sale)
        {
            // 1. Guard against null sale input
            if (sale == null) return;

            _activeSale = sale;

            // 2. Fresh initialization is safer than .Clear() to avoid null refs
            _productQueues = new Dictionary<int, CircularQueue<CustomerRequestBL>>();
            _userRequestStatus = new Dictionary<string, string>();

            // 3. Ensure fs_items exists before iterating
            if (_activeSale.fs_items == null) return;

            var node = _activeSale.fs_items.head;
            while (node != null)
            {
                var product = node.Data;

                // 4. Initialize a dedicated CircularQueue for each product
                // Using a capacity of 50 as per your previous requirement
                if (product != null && !_productQueues.ContainsKey(product.product_id))
                {
                    _productQueues.Add(product.product_id, new CircularQueue<CustomerRequestBL>(50));
                }

                node = node.Next;
            }

            AddLog($"[{DateTime.Now.ToLongTimeString()}] Live Context Set: {sale.title}");
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

            AddLog($"[{DateTime.Now.ToLongTimeString()}] User {customer.user_id} joined queue for {saleItem.name}");

            return "Queued";
        }

        // NEW: Call this method when the Sale Ends to process the priority heap
        public void FinalizeAllocations()
        {
            foreach (var productId in _productQueues.Keys)
            {
                var queue = _productQueues[productId];

                // Find product to check stock
                ProductBL liveProduct = null;
                var node = _activeSale.fs_items.head;
                while (node != null) 
                { 
                    if (node.Data.product_id == productId) 
                    { 
                        liveProduct = node.Data; break; 
                    } 
                    node = node.Next; 
                }

                if (liveProduct == null) continue;

                // Move items from CircularQueue to PriorityQueue (Allocation Heap)
                // STAGING: Move items from CircularQueue (Buffer) to PriorityQueue (Heap)
                // This sorts the users by their Join Time before allocation begins.
                while (!queue.IsEmpty())
                {
                    var request = queue.Dequeue();
                    string statusKey = $"{request.customer.user_id}_{productId}";

                    if (_userRequestStatus.ContainsKey(statusKey))
                    {
                        // HIGHEST PRIORITY = Earliest Ticks. 
                        // Using long.MaxValue - Ticks converts the earliest time into the largest number 
                        // for your Max-Heap implementation.
                        long priorityValue = long.MaxValue - request.requestTime.Ticks;
                        _activeSale.allocation_heap.Enqueue(request, (int)priorityValue);
                    }
                }

                // 3. ALLOCATION LOOP: 
                // This loop runs as long as there is stock AND people waiting in the heap.
                while (liveProduct.stock > 0 && !_activeSale.allocation_heap.IsEmpty())
                {
                    // Get the person with the highest priority (Earliest Join Time)
                    var winner = _activeSale.allocation_heap.Dequeue();
                    string winnerKey = $"{winner.customer.user_id}_{productId}";

                    // Only deduct stock if we are confirming a winner
                    liveProduct.stock--;
                    winner.allocated = true;
                    _userRequestStatus[winnerKey] = "Allocated";

                    AddLog($"[{DateTime.Now.ToLongTimeString()}] ✅ ALLOCATED: {winner.customer.user_id} gets {liveProduct.name}");
                }

                // 4. CLEANUP: Mark remaining people in the heap as Sold Out
                while (!_activeSale.allocation_heap.IsEmpty())
                {
                    var loser = _activeSale.allocation_heap.Dequeue();
                    string loserKey = $"{loser.customer.user_id}_{productId}";
                    _userRequestStatus[loserKey] = "Sold Out";
                }
            }
        }
        

        // --- UNJOIN LOGIC ---
        public void LeaveFlashSaleQueue(int userId, int productId)
        {
            string statusKey = $"{userId}_{productId}";

            if (_userRequestStatus.ContainsKey(statusKey))
            {
                _userRequestStatus.Remove(statusKey);
                AddLog($"[{DateTime.Now.ToLongTimeString()}] User {userId} LEFT queue for Product {productId}");
            }
        }

        private void ProcessAllocationForProduct(int productId)
        {
            if (!_productQueues.ContainsKey(productId)) return;
            var queue = _productQueues[productId];

            ProductBL liveProduct = null;
            var node = _activeSale.fs_items.head;
            while (node != null) { if (node.Data.product_id == productId) { liveProduct = node.Data; break; } node = node.Next; }

            if (liveProduct == null) return;

            while (!queue.IsEmpty())
            {
                CustomerRequestBL request = queue.Dequeue();
                string statusKey = $"{request.customer.user_id}_{productId}";

                // SKIP CHECK: If user removed from status map (Unjoined), skip allocation
                if (!_userRequestStatus.ContainsKey(statusKey)) continue;

                if (liveProduct.stock > 0)
                {
                    liveProduct.stock--;
                    request.allocated = true;
                    _activeSale.allocation_heap.Enqueue(request, 1);
                    if (_userRequestStatus.ContainsKey(statusKey)) _userRequestStatus[statusKey] = "Allocated";

                    AddLog($"[{DateTime.Now.ToLongTimeString()}] ✅ Allocation SUCCESS for User {request.customer.user_id}");
                }
                else
                {
                    request.allocated = false;
                    if (_userRequestStatus.ContainsKey(statusKey)) _userRequestStatus[statusKey] = "Sold Out";
                }
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
    }
}