using System;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Model_Classes
{
    public class ProductBL
    {
        public int product_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string[] images { get; set; }         // array of image URLs
        public double price { get; set; }
        public int stock { get; set; }

        // FIX: This is now an Object (CategoryBL), matching your JSON
        public CategoryBL category { get; set; }

        public MyLinkedList<string> tags { get; set; }

        public ProductBL()
        {
            product_id = 0;
            name = "";
            description = "";
            images = new string[0];
            price = 0;
            stock = 0;
            this.category = new CategoryBL();
            tags = new MyLinkedList<string>();
        }

        public ProductBL(int product_id, string name, string description, string[] images,
                         double price, int stock, CategoryBL category)
        {
            this.product_id = product_id;
            this.name = name;
            this.description = description;
            this.images = images;
            this.price = price;
            this.stock = stock;
            this.category = category;
            this.tags = new MyLinkedList<string>();
        }
    }
}