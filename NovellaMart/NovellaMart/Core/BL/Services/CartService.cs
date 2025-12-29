using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.BL.Data_Structures;
using NovellaMart.Core.DL;
using System;
using System.Threading.Tasks;

namespace NovellaMart.Core.BL.Services
{
    public class CartService
    {
        // Holds the state for the current user session
        private CartBL _activeCart = new CartBL();
        private const string CartFileName = "cart.json";

        public CartService()
        {
            // Initialize Cart: Try to load from file first
            LoadCartFromFile();

            // If file loading failed or returned null, ensure a fresh cart structure exists
            // REMOVED: Mock Data Seeding
            if (_activeCart == null)
            {
                _activeCart = new CartBL();
            }

            // Double check list initialization
            if (_activeCart.items == null)
            {
                _activeCart.items = new MyLinkedList<CartItemBL>();
            }

            // No initial Save needed if empty; save on first add.
        }

        // --- PERSISTENCE HELPER METHODS ---

        private void SaveCartToFile()
        {
            FileHandler.SaveData(CartFileName, _activeCart);
        }

        private void LoadCartFromFile()
        {
            var loadedCart = FileHandler.LoadData<CartBL>(CartFileName);
            if (loadedCart != null)
            {
                _activeCart = loadedCart;

                // Safety check: ensure the linked list is initialized if deserialization left it null
                if (_activeCart.items == null)
                {
                    _activeCart.items = new MyLinkedList<CartItemBL>();
                }
            }
        }

        // --- BUSINESS LOGIC (With Auto-Save) ---

        public void ClearCart()
        {
            // DSA: Reset the Linked List
            _activeCart.items = new MyLinkedList<CartItemBL>();
            SaveCartToFile(); // Persist change
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
                        RemoveItem(productId); // RemoveItem handles its own saving
                        return;
                    }
                    SaveCartToFile(); // Persist change
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
                    SaveCartToFile(); // Persist change
                    return;
                }
                current = current.Next;
                index++;
            }
        }

        public async Task AddToCart(ProductBL product, int quantity = 1)
        {
            if (product == null) return;

            var current = _activeCart.items.head;

            while (current != null)
            {
                if (current.Data.Product.product_id == product.product_id)
                {
                    current.Data.Quantity += quantity;
                    SaveCartToFile();
                    return;
                }
                current = current.Next;
            }

            _activeCart.items.InsertAtEnd(new CartItemBL(product, quantity));
            SaveCartToFile();
        }
    }
}