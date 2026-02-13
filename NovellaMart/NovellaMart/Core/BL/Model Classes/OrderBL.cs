using System;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Model_Classes
{
    public class OrderBL
    {
        public long order_id { get; set; }
        public CustomerBL customer { get; set; }
        public AddressBL address { get; set; }
        public MyLinkedList<CartItemBL> items { get; set; }
        public double totalAmount { get; set; }
        public DateTime orderTime { get; set; }
        public string status { get; set; }            // PENDING, CONFIRMED, SHIPPED, DELIVERED
        public string orderType { get; set; }         // Cash On Delivery, Card Payment
        public long flash_reservation_id { get; set; }

        public string customer_email
        {
            get => customer?.email;
            set
            {
                if (customer == null) customer = new CustomerBL();
                customer.email = value;
            }
        }

        public OrderBL()
        {
            order_id = DateTime.Now.Ticks;
            customer = new CustomerBL();
            address = new AddressBL();
            items = new MyLinkedList<CartItemBL>();
            totalAmount = 0.0;
            orderTime = DateTime.Now;
            status = "PENDING";
            orderType = "NORMAL";
            flash_reservation_id = 0;
        }

        public OrderBL(long id, CustomerBL cust, AddressBL addr, double total)
        {
            order_id = id;
            customer = cust;
            address = addr;
            items = new MyLinkedList<CartItemBL>();
            totalAmount = total;
            orderTime = DateTime.Now;
            status = "Confirmed";
        }
    }
}