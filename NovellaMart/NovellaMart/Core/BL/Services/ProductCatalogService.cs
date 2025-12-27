using System.Net.Http.Json;
using NovellaMart.Core.BL.Data_Structures;
using NovellaMart.Core.BL.Model_Classes;

namespace NovellaMart.Core.BL.Services
{
    public class ProductCatalogService
    {
        private readonly HttpClient _http;
        public MyLinkedList<ProductBL> MasterProductList { get; private set; }
        public AVLTreeGeneric<ProductBL> PriceTree { get; private set; }

        public ProductCatalogService(HttpClient http)
        {
            _http = http;
            MasterProductList = new MyLinkedList<ProductBL>();
            PriceTree = new AVLTreeGeneric<ProductBL>();
        }

        public int PriceComparator(ProductBL a, ProductBL b)
        {
            int priceComp = a.price.CompareTo(b.price);
            if (priceComp != 0) return priceComp;
            return a.product_id.CompareTo(b.product_id);
        }

        public async Task LoadProductsAsync()
        {
            if (MasterProductList.Count() > 0) return;

            try
            {
                var rawData = await _http.GetFromJsonAsync<List<ProductDto>>("sample-data/products.json");

                if (rawData != null)
                {
                    foreach (var item in rawData)
                    {
                        // 1. Map Categories
                        CategoryBL cat = new CategoryBL(0, item.category, 0); // Main Category
                        if (!string.IsNullOrEmpty(item.subCategory))
                        {
                            // We store SubCategory in the name, but track Parent via ID logic if needed
                            cat.name = item.subCategory;
                            cat.parent_category_id = 1;
                        }

                        // 2. Create Product
                        ProductBL product = new ProductBL(
                            item.id, item.name, "Description...", new string[] { item.imageUrl },
                            (double)item.price, 20, cat
                        );

                        // 3. Add Tags for Filtering (Main Cat, Sub Cat, Colors, Trending)
                        if (item.isTrending) product.tags.InsertAtEnd("Trending");
                        product.tags.InsertAtEnd(item.category); // Tag for Main Category (e.g., "Skincare")
                        if (!string.IsNullOrEmpty(item.subCategory)) product.tags.InsertAtEnd(item.subCategory); // Tag for Sub (e.g., "Serums")
                        foreach (var c in item.colors) product.tags.InsertAtEnd(c);

                        MasterProductList.InsertAtEnd(product);
                        PriceTree.Insert(product, PriceComparator);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        }

        public class ProductDto
        {
            public int id { get; set; }
            public string name { get; set; }
            public string category { get; set; }
            public string subCategory { get; set; }
            public decimal price { get; set; }
            public string imageUrl { get; set; }
            public List<string> colors { get; set; } = new();
            public bool isTrending { get; set; }
        }
    }
}