using System;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Model_Classes
{
    public class CartBL
    {
        public int cart_id { get; set; }
        public CustomerBL customer { get; set; }
        public MyLinkedList<CartItemBL> items { get; set; }   
        public PromoCodeBL AppliedPromoCode { get; set; }
        public double DiscountAmount { get; set; }

        public CartBL()
        {
            cart_id = 0;
            customer = null;
            items = new MyLinkedList<CartItemBL>();
        }

        public CartBL(int cart_id, CustomerBL customer)
        {
            this.cart_id = cart_id;
            this.customer = customer;
            items = new MyLinkedList<CartItemBL>();
        }

        
    }
}
