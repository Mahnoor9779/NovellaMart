using System;

namespace NovellaMart.Core.BL.Model_Classes
{
    internal class AddressBL
    {
        public int address_id { get; set; }
        public CustomerBL customer { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }

        public AddressBL()
        {
            address_id = 0;
            customer = null;
            street = "";
            city = "";
            postalCode = "";
            country = "";
        }

        public AddressBL(int address_id, CustomerBL customer, string street,
               string city, string postalCode, string country)
        {
            this.address_id = address_id;
            this.customer = customer;
            this.street = street;
            this.city = city;
            this.postalCode = postalCode;
            this.country = country;
        }
    }
}
