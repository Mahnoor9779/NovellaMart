using System.Collections.Generic;
using NovellaMart.Core.BL.Model_Classes;

namespace NovellaMart.Core.BL.Services
{
    public class FlashSaleCrudService
    {
        private static readonly List<FlashSaleBL> flashSales = new();
        private static int idCounter = 1;

        public void AddFlashSale(FlashSaleBL sale)
        {
            sale.flash_sale_id = idCounter++;
            sale.UpdateStatus();
            flashSales.Add(sale);
        }


        public void UpdateFlashSale(FlashSaleBL sale)
        {
            // reference updated automatically
        }

        public void DeleteFlashSale(int id)
        {
            flashSales.RemoveAll(s => s.flash_sale_id == id);
        }

        public List<FlashSaleBL> GetAllFlashSales()
        {
            foreach (var sale in flashSales)
            {
                sale.UpdateStatus();
            }

            return flashSales;
        }

    }
}
