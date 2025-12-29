using System;
using System.Collections.Generic;
using System.Linq;
using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.DL;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Services
{
    public class FlashSaleCrudService
    {
        private static List<FlashSaleBL> flashSales;
        private static int idCounter = 1;

        private const string FILE_NAME = "flash_sales.json";

        public FlashSaleCrudService()
        {
            if (flashSales == null)
            {
                flashSales = FileHandler.LoadData<List<FlashSaleBL>>(FILE_NAME) ?? new List<FlashSaleBL>();

                // Fix null references
                foreach (var sale in flashSales)
                {
                    sale.fs_items ??= new MyLinkedList<ProductBL>();
                    sale.request_queue ??= new CircularQueue<CustomerRequestBL>(100);
                    sale.allocation_heap ??= new PriorityQueue<CustomerRequestBL>();
                }

                if (flashSales.Count > 0)
                    idCounter = flashSales.Max(s => s.flash_sale_id) + 1;
            }
        }

        public void AddFlashSale(FlashSaleBL sale)
        {
            sale.flash_sale_id = idCounter++;
            sale.UpdateStatus();

            sale.fs_items ??= new MyLinkedList<ProductBL>();
            sale.request_queue ??= new CircularQueue<CustomerRequestBL>(100);
            sale.allocation_heap ??= new PriorityQueue<CustomerRequestBL>();

            flashSales.Add(sale);
            SaveToFile();
        }

        public void UpdateFlashSale(FlashSaleBL sale)
        {
            sale.UpdateStatus();
            SaveToFile();
        }

        public void DeleteFlashSale(int id)
        {
            flashSales.RemoveAll(s => s.flash_sale_id == id);
            SaveToFile();
        }

        public List<FlashSaleBL> GetAllFlashSales()
        {
            foreach (var sale in flashSales)
                sale.UpdateStatus();
            return flashSales;
        }

        private void SaveToFile()
        {
            FileHandler.SaveData(FILE_NAME, flashSales);
        }
    }
}
