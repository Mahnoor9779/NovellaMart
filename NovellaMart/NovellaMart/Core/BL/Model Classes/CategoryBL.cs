using System;
using System.Text.Json.Serialization;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Model_Classes
{
    public class CategoryBL
    {
        public int category_id { get; set; }
        public string name { get; set; }
        public int parent_category_id { get; set; }
        public MyLinkedList<CategoryBL> subcategories { get; set; }
        
        [JsonIgnore]
        public MyLinkedList<ProductBL> products { get; set; }
        public AVLTreeGeneric<CategoryBL> categoryTree { get; set; }

        public CategoryBL()
        {
            category_id = 0;
            name = "";
            parent_category_id = 0;
            subcategories = new MyLinkedList<CategoryBL>();
            products = new MyLinkedList<ProductBL>();
            categoryTree = new AVLTreeGeneric<CategoryBL>();
        }

        public CategoryBL(int category_id, string name, int parent_category_id)
        {
            this.category_id = category_id;
            this.name = name;
            this.parent_category_id = parent_category_id;
            subcategories = new MyLinkedList<CategoryBL>();
            products = new MyLinkedList<ProductBL>();
            categoryTree = new AVLTreeGeneric<CategoryBL>();
        }

        //public void insertSubcategory(CategoryBL c)
        //{
        //    subcategories.InsertAtEnd(c);
        //    categoryTree.Insert(this, (a, b) => a.category_id.CompareTo(b.category_id));
        //}

        
    }
}
