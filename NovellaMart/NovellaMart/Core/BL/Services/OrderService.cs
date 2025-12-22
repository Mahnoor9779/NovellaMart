using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Services
{
    public class OrderService
    {
        private readonly CartService _cartService;

        // Mock Database for Orders
        private static MyLinkedList<OrderBL> _allOrders = new MyLinkedList<OrderBL>();

        public OrderService(CartService cartService)
        {
            _cartService = cartService;
        }
        public MyLinkedList<OrderBL> GetAllOrders()
        {
            return _allOrders;
        }

        // Logic: Free in Lahore, increases with distance (Mock), Max 1000
        public double CalculateShipping(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return 0;

            string lowerCity = city.Trim().ToLower();

            if (lowerCity == "lahore") return 0; // Free delivery in Lahore

            // Mock Rates based on approx distance from Lahore
            var cityRates = new Dictionary<string, double>
            {
                { "sheikhupura", 100 },
                { "kasur", 150 },
                { "gujranwala", 180 },
                { "faisalabad", 200 }, // Explicit requirement
                { "fsd", 200 },
                { "sialkot", 250 },
                { "islamabad", 500 },
                { "rawalpindi", 500 },
                { "multan", 600 },
                { "peshawar", 800 },
                { "karachi", 1000 }, // Max cap
                { "quetta", 1000 }
            };

            if (cityRates.ContainsKey(lowerCity))
            {
                return cityRates[lowerCity];
            }

            // Default for other cities (capped at 1000)
            return 350;
        }

        public OrderBL PlaceOrder(string firstName, string lastName, string address, string city, string email, string paymentMethod, double shippingCost)
        {
            var cart = _cartService.GetCart();

            if (cart.items.head == null) return null;

            // 1. Create Address Object
            AddressBL shippingAddress = new AddressBL(0, null, address, city, "00000", "Pakistan");

            // 2. Create User/Customer Object
            CustomerBL customer = new CustomerBL();
            customer.firstName = firstName;
            customer.lastName = lastName;
            customer.email = email;

            // 3. Create Order Object
            OrderBL newOrder = new OrderBL();
            newOrder.order_id = DateTime.Now.Ticks;
            newOrder.customer = customer;
            newOrder.address = shippingAddress;
            newOrder.items = cart.items; // Transfer items

            // Total = Subtotal + Shipping
            newOrder.totalAmount = _cartService.GetTotalAmount() + shippingCost;

            newOrder.orderTime = DateTime.Now;
            newOrder.status = "Confirmed";
            newOrder.orderType = paymentMethod == "cod" ? "Cash On Delivery" : "Card Payment";

            // 4. Save to "Database"
            _allOrders.InsertAtEnd(newOrder);

            // 5. Clear Cart
            _cartService.ClearCart();

            return newOrder; // Return the object to generate the slip
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
    }
}