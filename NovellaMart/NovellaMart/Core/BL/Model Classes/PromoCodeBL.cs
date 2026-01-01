using System;

namespace NovellaMart.Core.BL.Model_Classes
{
    public class PromoCodeBL
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int DiscountPercentage { get; set; }
        public bool IsActive { get; set; } = true;

        public PromoCodeBL() { }

        public PromoCodeBL(int id, string code, int discount)
        {
            Id = id;
            Code = code;
            DiscountPercentage = discount;
            IsActive = true;
        }
    }
}
