using System;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Model_Classes
{
    public class OrderBL
    {
        public long order_id { get; set; }
        public CustomerBL customer { get; set; }
        public AddressBL address { get; set; }
        public MyLinkedList<CartItemBL> items { get; set; }   // products + quantities in a single list
        public double totalAmount { get; set; }
        public DateTime orderTime { get; set; }
        public string status { get; set; }           // PENDING, PACKED, SHIPPED, DELIVERED, CANCELLED
        public string orderType { get; set; }        // NORMAL, FLASH_SALE
        public long flash_reservation_id { get; set; } // 0 if N/A

        public OrderBL()
        {
            order_id = 0;
            customer = null;
            address = null;
            items = new MyLinkedList<CartItemBL>();
            totalAmount = 0.0;
            orderTime = DateTime.Now;
            status = "PENDING";
            orderType = "NORMAL";
            flash_reservation_id = 0;
        }

        public OrderBL(long order_id, CustomerBL customer, AddressBL address, double totalAmount)
        {
            this.order_id = order_id;
            this.customer = customer;
            this.address = address;
            items = new MyLinkedList<CartItemBL>();
            this.totalAmount = totalAmount;
            orderTime = DateTime.Now;
            status = "PENDING";
            orderType = "NORMAL";
            flash_reservation_id = 0;
        }
    }
}
