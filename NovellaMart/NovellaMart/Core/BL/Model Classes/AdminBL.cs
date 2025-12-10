using System;

namespace NovellaMart.Core.BL.Model_Classes
{
    internal class AdminBL : UserBL
    {
        public AdminBL() : base()
        {
            this.role = "Admin";
        }

        public AdminBL(int user_id, string username, string email, string password,
                       string firstName, string lastName, string profilePicture)
            : base(user_id, username, email, password, "Admin", firstName, lastName, profilePicture)
        {
        }
    }
}
