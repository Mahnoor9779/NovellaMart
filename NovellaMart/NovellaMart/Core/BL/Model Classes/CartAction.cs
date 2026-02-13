using System.Collections.Generic;

namespace NovellaMart.Core.BL.Model_Classes
{
    public class CartAction
    {
        public string Type { get; set; } = "";
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public List<CartItemBL>? RestoredItems { get; set; }
    }
}

