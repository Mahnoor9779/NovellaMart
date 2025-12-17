using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Services
{
    public class FlashSaleService
    {
        private FlashSaleBL _activeSale;
        private Dictionary<int, CircularQueue<CustomerRequestBL>> _productQueues;
        private Dictionary<string, string> _userRequestStatus;

        public FlashSaleService()
        {
            InitializeMockSale();
        }

        private void InitializeMockSale()
        {
            _activeSale = new FlashSaleBL();
            _activeSale.flash_sale_id = 101;
            _activeSale.title = "Midnight Tech Rush";
            _activeSale.status = "ACTIVE";
            _activeSale.startTime = DateTime.Now;
            _activeSale.endTime = DateTime.Now.AddHours(2);

            _activeSale.allocation_heap = new PriorityQueue<CustomerRequestBL>();

            _productQueues = new Dictionary<int, CircularQueue<CustomerRequestBL>>();
            _userRequestStatus = new Dictionary<string, string>();

            var catTech = new CategoryBL(5, "Tech", 0);
            var p1 = new ProductBL(999, "Gaming Headset", "Pro Noise Cancelling", new[] { "https://placehold.co/200" }, 5000.00, 3, catTech);
            var p2 = new ProductBL(888, "Smart Watch", "Fitness Tracker", new[] { "https://placehold.co/200" }, 3000.00, 10, catTech);

            if (_activeSale.fs_items == null) _activeSale.fs_items = new MyLinkedList<ProductBL>();
            _activeSale.fs_items.InsertAtEnd(p1);
            _activeSale.fs_items.InsertAtEnd(p2);

            var node = _activeSale.fs_items.head;
            while (node != null)
            {
                var product = node.Data;
                _productQueues.Add(product.product_id, new CircularQueue<CustomerRequestBL>(50));
                node = node.Next;
            }
        }

        public FlashSaleBL GetActiveSale() => _activeSale;

        public string JoinFlashSaleQueue(CustomerBL customer, int productId)
        {
            if (_activeSale.status != "ACTIVE") return "Sale Ended";

            string statusKey = $"{customer.user_id}_{productId}";

            if (_userRequestStatus.ContainsKey(statusKey))
            {
                return _userRequestStatus[statusKey];
            }

            if (!_productQueues.ContainsKey(productId))
            {
                return "Product Not in Sale";
            }

            var targetQueue = _productQueues[productId];

            ProductBL saleItem = null;
            var node = _activeSale.fs_items.head;
            while (node != null)
            {
                if (node.Data.product_id == productId)
                {
                    saleItem = node.Data;
                    break;
                }
                node = node.Next;
            }

            CustomerRequestBL request = new CustomerRequestBL();
            request.request_id = DateTime.Now.Ticks;
            request.customer = customer;
            request.product = saleItem;
            request.requestTime = DateTime.Now;
            request.allocated = false;

            if (targetQueue.IsFull())
            {
                return "Queue Full";
            }

            targetQueue.Enqueue(request);
            _userRequestStatus.Add(statusKey, "Queued");

            ProcessAllocationForProduct(productId);

            return _userRequestStatus[statusKey];
        }

        private void ProcessAllocationForProduct(int productId)
        {
            if (!_productQueues.ContainsKey(productId)) return;

            var queue = _productQueues[productId];

            ProductBL liveProduct = null;
            var node = _activeSale.fs_items.head;
            while (node != null)
            {
                if (node.Data.product_id == productId)
                {
                    liveProduct = node.Data;
                    break;
                }
                node = node.Next;
            }

            if (liveProduct == null) return;

            while (!queue.IsEmpty())
            {
                CustomerRequestBL request = queue.Dequeue();
                string statusKey = $"{request.customer.user_id}_{productId}";

                if (liveProduct.stock > 0)
                {
                    liveProduct.stock--;
                    request.allocated = true;

                    _activeSale.allocation_heap.Enqueue(request, 1);

                    if (_userRequestStatus.ContainsKey(statusKey))
                        _userRequestStatus[statusKey] = "Allocated";
                }
                else
                {
                    request.allocated = false;

                    if (_userRequestStatus.ContainsKey(statusKey))
                        _userRequestStatus[statusKey] = "Sold Out";
                }
            }
        }

        public string GetUserStatus(int userId, int productId)
        {
            string key = $"{userId}_{productId}";
            if (_userRequestStatus.ContainsKey(key)) return _userRequestStatus[key];
            return "None";
        }

        // --- NEW METHOD TO FIX ERROR ---
        public int GetQueuePosition(int userId, int productId)
        {
            // Since CircularQueue doesn't easily support finding specific index without iteration,
            // we return the current total size of the queue for that product as the "Queue Position".
            // This represents that the user is effectively at the end of the current line.
            if (_productQueues.ContainsKey(productId))
            {
                return _productQueues[productId].Size();
            }
            return 0;
        }
    }
}