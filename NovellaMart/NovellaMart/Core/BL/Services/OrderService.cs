using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.BL.Data_Structures;
using NovellaMart.Core.DL;

namespace NovellaMart.Core.BL.Services
{
    public class OrderService
    {
        private readonly CartService _cartService;
        private const string FilePath = "orders.json";
        private static MyLinkedList<OrderBL> _allOrders;

        public OrderService(CartService cartService)
        {
            _cartService = cartService;

            if (_allOrders == null)
            {
                _allOrders = FileHandler.LoadData<MyLinkedList<OrderBL>>(FilePath);
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

        public OrderBL PlaceOrder(string firstName, string lastName, string address, string city, string email, string paymentMethod, double shippingCost)
        {
            var cart = _cartService.GetCart();
            if (cart.items.head == null) return null;

            AddressBL shippingAddress = new AddressBL(0, null, address, city, "00000", "Pakistan");

            CustomerBL customer = new CustomerBL();
            customer.firstName = firstName;
            customer.lastName = lastName;
            customer.email = email;

            OrderBL newOrder = new OrderBL();
            newOrder.order_id = DateTime.Now.Ticks;
            newOrder.customer = customer;
            newOrder.address = shippingAddress;

            // --- FIX: DEEP COPY ITEMS ---
            // We must create a NEW list for the order. If we just say 'newOrder.items = cart.items',
            // emptying the cart later would also empty this order record!
            newOrder.items = new MyLinkedList<CartItemBL>();

            var cartNode = cart.items.head;
            while (cartNode != null)
            {
                // Add item to Order History
                newOrder.items.InsertAtEnd(cartNode.Data);
                cartNode = cartNode.Next;
            }

            newOrder.totalAmount = _cartService.GetTotalAmount() + shippingCost;
            newOrder.orderTime = DateTime.Now;
            newOrder.status = "Confirmed";
            newOrder.orderType = paymentMethod == "cod" ? "Cash On Delivery" : "Card Payment";

            // Save Order to History
            _allOrders.InsertAtEnd(newOrder);
            FileHandler.SaveData(FilePath, _allOrders);

            // --- REMOVE PRODUCTS FROM CART ---
            // Requirement: "Remove those particular products from the cart"
            // We iterate through the items we just ordered and remove them one by one.
            var orderNode = newOrder.items.head;
            while (orderNode != null)
            {
                int productId = orderNode.Data.Product.product_id;
                _cartService.RemoveItem(productId);
                orderNode = orderNode.Next;
            }

            return newOrder;
        }
    }
}