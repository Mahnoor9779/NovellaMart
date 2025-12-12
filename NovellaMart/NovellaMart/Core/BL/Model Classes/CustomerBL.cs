using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Model_Classes
{
    public class CustomerBL : UserBL
    {
        public string contact { get; set; }
        public MyLinkedList<AddressBL> Addresses { get; set; }
        public MyLinkedList<OrderBL> OrderHistory { get; set; }
        public CartBL Cart { get; set; }

        public CustomerBL() : base()
        {
            this.role = "Customer";
            contact = "";
            Addresses = new MyLinkedList<AddressBL>();
            OrderHistory = new MyLinkedList<OrderBL>();
            Cart = new CartBL();
        }

        public CustomerBL(int user_id, string username, string email, string password,
                          string firstName, string lastName, string profilePicture, string contact)
            : base(user_id, username, email, password, "Customer", firstName, lastName, profilePicture)
        {
            this.contact = contact;
            Addresses = new MyLinkedList<AddressBL>();
            OrderHistory = new MyLinkedList<OrderBL>();
            Cart = new CartBL();
        }
    }
}
