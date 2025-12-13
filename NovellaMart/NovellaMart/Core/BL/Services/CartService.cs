using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Services
{
    public class CartService
    {
        // Holds the state for the current user session
        private CartBL _activeCart = new CartBL();

        public CartService()
        {
            // TEMPORARY: Initialize Mock Data
            if (_activeCart.items == null || _activeCart.items.head == null)
            {
                if (_activeCart.items == null) _activeCart.items = new MyLinkedList<CartItemBL>();

                var catSkin = new CategoryBL(1, "Skin Care", 0);
                var catHair = new CategoryBL(2, "Hair Care", 0);

                var p1 = new ProductBL(101, "SilkSculpt Serum", "Hydrating serum", new[] { "https://placehold.co/100" }, 35.00, 50, catSkin);
                var p2 = new ProductBL(102, "Argan Glow Oil", "Shine for hair", new[] { "https://placehold.co/100" }, 63.00, 20, catHair);
                var p3 = new ProductBL(103, "Rose Mist", "Refreshing mist", new[] { "https://placehold.co/100" }, 15.00, 100, catSkin);

                _activeCart.items.InsertAtEnd(new CartItemBL(p1, 2));
                _activeCart.items.InsertAtEnd(new CartItemBL(p2, 1));
                _activeCart.items.InsertAtEnd(new CartItemBL(p3, 3));
            }
        }

        // --- BUSINESS LOGIC ---

        public void ClearCart()
        {
            // DSA: Reset the Linked List
            _activeCart.items = new MyLinkedList<CartItemBL>();
        }

        public CartBL GetCart()
        {
            return _activeCart;
        }

        public double GetTotalAmount()
        {
            double total = 0;
            var current = _activeCart.items.head;

            while (current != null)
            {
                total += (current.Data.Product.price * current.Data.Quantity);
                current = current.Next;
            }
            return total;
        }

        public void UpdateQuantity(int productId, int change)
        {
            var current = _activeCart.items.head;

            while (current != null)
            {
                if (current.Data.Product.product_id == productId)
                {
                    int newQty = current.Data.Quantity + change;
                    if (newQty > 0)
                    {
                        current.Data.Quantity = newQty;
                    }
                    else
                    {
                        RemoveItem(productId);
                    }
                    return;
                }
                current = current.Next;
            }
        }

        public void RemoveItem(int productId)
        {
            int index = 0;
            var current = _activeCart.items.head;

            while (current != null)
            {
                if (current.Data.Product.product_id == productId)
                {
                    _activeCart.items.DeleteAtIndex(index);
                    return;
                }
                current = current.Next;
                index++;
            }
        }

        public void AddToCart(ProductBL product, int quantity = 1)
        {
            if (product == null) return;

            var current = _activeCart.items.head;

            // If product already exists → increase quantity
            while (current != null)
            {
                if (current.Data.Product.product_id == product.product_id)
                {
                    current.Data.Quantity += quantity;
                    return;
                }
                current = current.Next;
            }

            // Else add new item
            _activeCart.items.InsertAtEnd(new CartItemBL(product, quantity));
        }
    }
}