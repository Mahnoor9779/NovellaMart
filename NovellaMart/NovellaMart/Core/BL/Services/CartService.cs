using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.BL.Data_Structures;
using NovellaMart.Core.DL;
using System;
using System.Threading.Tasks;

namespace NovellaMart.Core.BL.Services
{
    public class CartService
    {
        private CartBL _activeCart = new CartBL();
        private const string CartFileName = "cart.json";

        public CartService()
        {
            LoadCartFromFile();

            if (_activeCart == null)
            {
                _activeCart = new CartBL();
            }

            if (_activeCart.items == null)
            {
                _activeCart.items = new MyLinkedList<CartItemBL>();
            }

        }


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

                if (_activeCart.items == null)
                {
                    _activeCart.items = new MyLinkedList<CartItemBL>();
                }
            }
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

            if (_activeCart.AppliedPromoCode != null)
            {
                double discount = (total * _activeCart.AppliedPromoCode.DiscountPercentage) / 100.0;
                _activeCart.DiscountAmount = discount;
                return total - discount;
            }
            
            _activeCart.DiscountAmount = 0;
            return total;
        }

        public void ApplyPromoCode(PromoCodeBL promo)
        {
            if(promo != null && promo.IsActive)
            {
                _activeCart.AppliedPromoCode = promo;
                SaveCartToFile();
            }
        }

        public void RemovePromoCode()
        {
            _activeCart.AppliedPromoCode = null;
            _activeCart.DiscountAmount = 0;
            SaveCartToFile();
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
                        return;
                    }
                    SaveCartToFile();
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
                    SaveCartToFile();
                    return;
                }
                current = current.Next;
                index++;
            }
        }

        private StackArray<CartAction> _undoStack = new StackArray<CartAction>(50);

        public async Task AddToCart(ProductBL product, int quantity = 1)
        {
            if (product == null) return;

            var current = _activeCart.items.head;

            while (current != null)
            {
                if (current.Data.Product.product_id == product.product_id)
                {
                    current.Data.Quantity += quantity;
                    _undoStack.Push(new CartAction { Type = "Add", ProductId = product.product_id, Quantity = quantity });
                    SaveCartToFile();
                    return;
                }
                current = current.Next;
            }

            _activeCart.items.InsertAtEnd(new CartItemBL(product, quantity));
            _undoStack.Push(new CartAction { Type = "Add", ProductId = product.product_id, Quantity = quantity });
            SaveCartToFile();
        }

        public void ClearCart()
        {
            List<CartItemBL> itemsBackup = new List<CartItemBL>();
            var node = _activeCart.items.head;
            while (node != null)
            {
                itemsBackup.Add(node.Data);
                node = node.Next;
            }

            _undoStack.Push(new CartAction { Type = "Clear", RestoredItems = itemsBackup });

            _activeCart.items = new MyLinkedList<CartItemBL>();
            SaveCartToFile();
        }

        public void UndoLastAction()
        {
            if (_undoStack.IsEmpty()) return;

            var action = _undoStack.Pop();

            if (action.Type == "Add")
            {
                UpdateQuantity(action.ProductId, -action.Quantity);
            }
            else if (action.Type == "Clear")
            {
                _activeCart.items = new MyLinkedList<CartItemBL>();
                foreach (var item in action.RestoredItems)
                {
                    _activeCart.items.InsertAtEnd(item);
                }
                SaveCartToFile();
            }
        }

        public bool CanUndo()
        {
            return !_undoStack.IsEmpty();
        }

        public CartAction PeekLastAction()
        {
           return _undoStack.Peek();
        }

        public int GetStackSize()
        {
            return _undoStack.Size();
        }
    }
}