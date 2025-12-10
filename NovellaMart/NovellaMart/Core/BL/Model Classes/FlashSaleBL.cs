using System;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Model_Classes
{
    internal class FlashSaleBL
    {
        public int flash_sale_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string status { get; set; }           // SCHEDULED, ACTIVE, ENDED, CANCELLED
        public MyLinkedList<ProductBL> fs_items { get; set; }
        public CircularQueue<CustomerRequestBL> request_queue { get; set; }
        public PriorityQueue<CustomerRequestBL> allocation_heap { get; set; }

        public FlashSaleBL()
        {
            flash_sale_id = 0;
            title = "";
            description = "";
            startTime = DateTime.Now;
            endTime = DateTime.Now;
            status = "DRAFT";
            fs_items = new MyLinkedList<ProductBL>();
            request_queue = new CircularQueue<CustomerRequestBL>(100);
            allocation_heap = new PriorityQueue<CustomerRequestBL>();
        }

        public FlashSaleBL(int flash_sale_id, string title, DateTime startTime, DateTime endTime)
        {
            this.flash_sale_id = flash_sale_id;
            this.title = title;
            this.description = "";
            this.startTime = startTime;
            this.endTime = endTime;
            this.status = "SCHEDULED";
            fs_items = new MyLinkedList<ProductBL>();
            request_queue = new CircularQueue<CustomerRequestBL>(100);
            allocation_heap = new PriorityQueue<CustomerRequestBL>();
        }
    }
}
