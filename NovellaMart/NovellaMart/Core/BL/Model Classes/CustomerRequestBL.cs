using System;

namespace NovellaMart.Core.BL.Model_Classes
{
    public class CustomerRequestBL
    {
        public long request_id { get; set; }
        public CustomerBL customer { get; set; }
        public ProductBL product { get; set; }
        public DateTime requestTime { get; set; }
        public bool allocated { get; set; }

        public CustomerRequestBL()
        {
            request_id = 0;
            customer = null;
            product = null;
            requestTime = DateTime.Now;
            allocated = false;
        }

        public CustomerRequestBL(long request_id, CustomerBL customer, ProductBL product)
        {
            this.request_id = request_id;
            this.customer = customer;
            this.product = product;
            requestTime = DateTime.Now;
            allocated = false;
        }
    }
}
