
using NovellaMart.Core.BL.Model_Classes;

//FlashSale Allocation Service
namespace NovellaMart.Core.BL.Services
{
    public class FlashSaleAllocationService
    {
        private readonly FlashSaleService _flashSaleService;
        private readonly FlashSaleCrudService _crudService;

        public FlashSaleAllocationService(FlashSaleService flashSaleService, FlashSaleCrudService crudService)
        {
            _flashSaleService = flashSaleService;
            _crudService = crudService;
        }

        // Get products for a SPECIFIC sale
        public List<ProductBL> GetSaleProducts(int saleId)
        {
            var sale = _crudService.GetAllFlashSales().FirstOrDefault(s => s.flash_sale_id == saleId);
            var list = new List<ProductBL>();
            var node = sale?.fs_items?.head;
            while (node != null)
            {
                list.Add(node.Data);
                node = node.Next;
            }
            return list;
        }

        // Get results from the allocation heap
        public List<CustomerRequestBL> GetAllocationsForProduct(int saleId, int productId)
        {
            return _flashSaleService.GetQueueSnapshotAllocation(productId);
        }

        public int GetQueueCountForProduct(int productId)
        {
            return _flashSaleService.GetQueueCount(productId);
        }
    }
}
