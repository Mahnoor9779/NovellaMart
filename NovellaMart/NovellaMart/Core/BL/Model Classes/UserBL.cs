using System;

namespace NovellaMart.Core.BL.Model_Classes
{
    public class UserBL
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string profilePicture { get; set; }
        public DateTime createdAt { get; set; }
        public bool isActive { get; set; }

        // Contact Fields
        public string phoneNumber { get; set; } = "";
        public string address { get; set; } = "";
        public string city { get; set; } = "";
        public string zipCode { get; set; } = "";

        // 1. Default Constructor
        public UserBL()
        {
            // Generate a random 6-digit ID (Simple approach)
            user_id = new Random().Next(100000, 999999);
            username = "";
            email = "";
            password = "";
            role = "Member";
            firstName = "";
            lastName = "";
            profilePicture = "";
            createdAt = DateTime.Now;
            isActive = true;
        }

        // 2. 👇 ADD THIS CONSTRUCTOR BACK (This fixes the AdminBL error)
        public UserBL(int user_id, string username, string email, string password,
                      string role, string firstName, string lastName, string profilePicture)
        {
            this.user_id = user_id;
            this.username = username;
            this.email = email;
            this.password = password;
            this.role = role;
            this.firstName = firstName;
            this.lastName = lastName;
            this.profilePicture = profilePicture;
            this.createdAt = DateTime.Now;
            this.isActive = true;
        }
    }
}