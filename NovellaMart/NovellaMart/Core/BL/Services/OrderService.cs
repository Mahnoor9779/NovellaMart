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
        private const string FilePath = "orders";
        private static MyLinkedList<OrderBL> _allOrders;
        private readonly ProductCatalogService _catalogService;

        public OrderService(CartService cartService, ProductCatalogService catalogService)
        {
            _cartService = cartService;
            _catalogService = catalogService;

            if (_allOrders == null)
            {
                try
                {
                    _allOrders = FileHandler.LoadData<MyLinkedList<OrderBL>>(FilePath);
                }
                catch
                {
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

            if (lowerCity == "lahore") return 0;

            var cityRates = new Dictionary<string, double>
            {
                { "sheikhupura", 100 }, { "kasur", 150 }, { "gujranwala", 180 },
                { "faisalabad", 200 }, { "fsd", 200 }, { "sialkot", 250 },
                { "islamabad", 500 }, { "rawalpindi", 500 }, { "multan", 600 },
                { "peshawar", 800 }, { "karachi", 1000 }, { "quetta", 1000 }
            };

            if (cityRates.ContainsKey(lowerCity)) return cityRates[lowerCity];

            return 350;
        }

        public OrderBL PlaceOrder(int userId, string firstName, string lastName, string addressStr, string city, string email, string paymentMethod, double shippingCost)
        {
            var cart = _cartService.GetCart();
            if (cart.items.head == null) return null;

            AddressBL shippingAddress = new AddressBL(0, null, addressStr, city, "00000", "Pakistan");

            CustomerBL customer = new CustomerBL();
            customer.user_id = userId;
            customer.firstName = firstName;
            customer.lastName = lastName;
            customer.email = email;

            OrderBL newOrder = new OrderBL();
            newOrder.order_id = DateTime.Now.Ticks;
            newOrder.customer = customer;
            newOrder.address = shippingAddress;
            newOrder.totalAmount = _cartService.GetTotalAmount() + shippingCost;
            newOrder.orderTime = DateTime.Now;
            newOrder.status = "Confirmed";
            newOrder.orderType = paymentMethod == "cod" ? "Cash On Delivery" : "Card Payment";

            newOrder.items = new MyLinkedList<CartItemBL>();
            var cartNode = cart.items.head;
            while (cartNode != null)
            {
                newOrder.items.InsertAtEnd(cartNode.Data);
                cartNode = cartNode.Next;
            }

            var orderNode = newOrder.items.head;
            while (orderNode != null)
            {
                int productId = orderNode.Data.Product.product_id;
                int purchasedQty = orderNode.Data.Quantity;

                var masterProduct = _catalogService.MasterProductList.FirstOrDefault(p => p.product_id == productId);
                if (masterProduct != null)
                {
                    masterProduct.stock -= purchasedQty;
                }
                _cartService.RemoveItem(productId);
                orderNode = orderNode.Next;
            }

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