using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Services
{
    public class FlashSaleAllocationService
    {
        private readonly FlashSaleService _flashSaleService;

        public FlashSaleAllocationService(FlashSaleService flashSaleService)
        {
            _flashSaleService = flashSaleService;
        }

        public List<ProductBL> GetSaleProducts()
        {
            var sale = _flashSaleService.GetActiveSale();
            var list = new List<ProductBL>();

            var node = sale?.fs_items?.head;
            while (node != null)
            {
                list.Add(node.Data);
                node = node.Next;
            }

            return list;
        }

        public List<CustomerRequestBL> GetAllocationsForProduct(int productId)
        {
            var sale = _flashSaleService.GetActiveSale();
            var results = new List<CustomerRequestBL>();

            if (sale?.allocation_heap == null)
                return results;

            var heapCopy = sale.allocation_heap.Clone();

            while (!heapCopy.IsEmpty())
            {
                var req = heapCopy.Dequeue();
                if (req.product.product_id == productId)
                    results.Add(req);
            }

            return results;
        }

        public int GetQueueCountForProduct(int productId)
        {
            return _flashSaleService.GetQueueCount(productId);
        }


    }
}
