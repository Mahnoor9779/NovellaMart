using System;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Model_Classes
{
    internal class CartItemBL
    {
        public ProductBL Product { get; set; }
        public int Quantity { get; set; }

        public CartItemBL()
        {
            Product = null;
            Quantity = 0;
        }

        public CartItemBL(ProductBL product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }
}
