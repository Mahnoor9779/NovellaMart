using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.BL.Data_Structures;
using NovellaMart.Core.BL.Services;
using NovellaMart.Core.DL;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace NovellaMart.Core.BL.Services
{
    public class OrderService
    {
        private readonly CartService _cartService;
        private const string FilePath = "Core/DL/orders.json"; // Ensure path is correct
        private static MyLinkedList<OrderBL> _allOrders;
        private readonly ProductCatalogService _catalogService;

        public OrderService(CartService cartService, ProductCatalogService catalogService)
        {
            _cartService = cartService;
            _catalogService = catalogService;

            if (_allOrders == null)
            {
                // Try to load using your FileHandler
                try
                {
                    _allOrders = FileHandler.LoadData<MyLinkedList<OrderBL>>(FilePath);
                }
                catch
                {
                    // Fallback if file doesn't exist or handler fails
                    _allOrders = new MyLinkedList<OrderBL>();
                }

                if (_allOrders == null)
                {
                    _allOrders = new MyLinkedList<OrderBL>();
                }
            }
        }

        public MyLinkedList<OrderBL> GetAllOrders()
        {
            return _allOrders;
        }

        public OrderBL GetOrderById(long id)
        {
            var current = _allOrders.head;
            while (current != null)
            {
                if (current.Data.order_id == id)
                {
                    return current.Data;
                }
                current = current.Next;
            }
            return null;
        }

        public double CalculateShipping(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return 0;
            string lowerCity = city.Trim().ToLower();

            // Lahore is Free
            if (lowerCity == "lahore") return 0;

            var cityRates = new Dictionary<string, double>
            {
                { "sheikhupura", 100 }, { "kasur", 150 }, { "gujranwala", 180 },
                { "faisalabad", 200 }, { "fsd", 200 }, { "sialkot", 250 },
                { "islamabad", 500 }, { "rawalpindi", 500 }, { "multan", 600 },
                { "peshawar", 800 }, { "karachi", 1000 }, { "quetta", 1000 }
            };

            if (cityRates.ContainsKey(lowerCity)) return cityRates[lowerCity];

            // Default rate for other cities
            return 350;
        }

        public OrderBL PlaceOrder(string firstName, string lastName, string addressStr, string city, string email, string paymentMethod, double shippingCost)
        {
            var cart = _cartService.GetCart();
            if (cart.items.head == null) return null; // Cannot place empty order

            // 1. Create Address & Customer Objects
            AddressBL shippingAddress = new AddressBL(0, null, addressStr, city, "00000", "Pakistan");

            CustomerBL customer = new CustomerBL();
            customer.firstName = firstName;
            customer.lastName = lastName;
            customer.email = email;

            // 2. Create Order Object
            OrderBL newOrder = new OrderBL();
            newOrder.order_id = DateTime.Now.Ticks; // Use Ticks for unique Long ID
            newOrder.customer = customer;
            newOrder.address = shippingAddress;
            newOrder.totalAmount = _cartService.GetTotalAmount() + shippingCost;
            newOrder.orderTime = DateTime.Now;
            newOrder.status = "Confirmed";
            newOrder.orderType = paymentMethod == "cod" ? "Cash On Delivery" : "Card Payment";

            // 3. Deep Copy Items (Move from Cart to Order)
            newOrder.items = new MyLinkedList<CartItemBL>();
            var cartNode = cart.items.head;
            while (cartNode != null)
            {
                newOrder.items.InsertAtEnd(cartNode.Data);
                cartNode = cartNode.Next;
            }

            // 4. Clear Purchased Items from Cart
            var orderNode = newOrder.items.head;
            while (orderNode != null)
            {
                int productId = orderNode.Data.Product.product_id;
                int purchasedQty = orderNode.Data.Quantity;

                // 1. Locate the product in the master list to permanently reduce stock
                // Assuming CatalogService is available or using a shared static list

                var masterProduct = _catalogService.MasterProductList.FirstOrDefault(p => p.product_id == productId);
                if (masterProduct != null)
                {
                    masterProduct.stock -= purchasedQty; // Decrement master stock
                }
                _cartService.RemoveItem(productId);
                orderNode = orderNode.Next;
            }

            // 5. Persist the updated stock and the new order [cite: 165]
            FileHandler.SaveData("sample-data/products.json", _catalogService.MasterProductList);

            _allOrders.InsertAtEnd(newOrder);
            FileHandler.SaveData(FilePath, _allOrders);

            return newOrder;
        
        }

        public MyLinkedList<OrderBL> GetOrdersByEmail(string email)
        {
            var userOrders = new MyLinkedList<OrderBL>();
            if (string.IsNullOrEmpty(email)) return userOrders;

            var current = _allOrders.head;
            while (current != null)
            {
                // Safely check email match (case-insensitive)
                string orderEmail = current.Data.customer?.email;

                if (!string.IsNullOrEmpty(orderEmail) &&
                    orderEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    userOrders.InsertAtEnd(current.Data);
                }
                current = current.Next;
            }
            return userOrders;
        }
    }
}