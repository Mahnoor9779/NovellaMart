using System;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Model_Classes
{
    internal class StoreProfileBL
    {
        public int store_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string timings { get; set; }
        public string contact { get; set; }
        public string email { get; set; }
        public AdminBL storeManager { get; set; }

        public StoreProfileBL()
        {
            store_id = 0;
            name = "";
            description = "";
            timings = "";
            contact = "";
            email = "";
            storeManager = null; 
        }

        public StoreProfileBL(int store_id, string name, string description,
                              string timings, string contact, string email,
                              AdminBL storeManager)
        {
            this.store_id = store_id;
            this.name = name;
            this.description = description;
            this.timings = timings;
            this.contact = contact;
            this.email = email;
            this.storeManager = storeManager;
        }
    }
}
