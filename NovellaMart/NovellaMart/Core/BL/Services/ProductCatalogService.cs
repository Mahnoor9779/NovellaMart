using System.Net.Http.Json;
using NovellaMart.Core.BL.Data_Structures;
using NovellaMart.Core.BL.Model_Classes;

namespace NovellaMart.Core.BL.Services
{
    public class ProductCatalogService
    {
        private readonly HttpClient _http;

        public MyLinkedList<ProductBL> MasterProductList { get; private set; }
        public MyLinkedList<CategoryBL> MasterCategoryList { get; private set; }
        public AVLTreeGeneric<ProductBL> PriceTree { get; private set; }

        public ProductCatalogService(HttpClient http)
        {
            _http = http;
            MasterProductList = new MyLinkedList<ProductBL>();
            MasterCategoryList = new MyLinkedList<CategoryBL>();
            PriceTree = new AVLTreeGeneric<ProductBL>();
        }

        /* ===========================
           Price Comparator
           =========================== */
        public int PriceComparator(ProductBL a, ProductBL b)
        {
            int priceComp = a.price.CompareTo(b.price);
            if (priceComp != 0) return priceComp;
            return a.product_id.CompareTo(b.product_id);
        }

        /* ===========================
           Load Products from JSON
           =========================== */
        public async Task LoadProductsAsync()
        {
            if (MasterProductList.Count() > 0)
                return;

            var rawData = await _http.GetFromJsonAsync<List<ProductDto>>(
                "sample-data/products.json"
            );

            if (rawData == null) return;

            int categoryIdCounter = 1;
            int subCategoryIdCounter = 100;

            foreach (var item in rawData)
            {
                /* ===========================
                   MAIN CATEGORY
                   =========================== */
                CategoryBL mainCategory = FindCategoryByName(item.category);

                if (mainCategory == null)
                {
                    mainCategory = new CategoryBL(categoryIdCounter++, item.category, 0);
                    MasterCategoryList.InsertAtEnd(mainCategory);
                }

                /* ===========================
                   SUB CATEGORY
                   =========================== */
                CategoryBL finalCategory = mainCategory;

                if (!string.IsNullOrWhiteSpace(item.subCategory))
                {
                    CategoryBL subCategory = FindSubCategory(
                        mainCategory,
                        item.subCategory
                    );

                    if (subCategory == null)
                    {
                        subCategory = new CategoryBL(
                            subCategoryIdCounter++,
                            item.subCategory,
                            mainCategory.category_id
                        );

                        mainCategory.subcategories.InsertAtEnd(subCategory);
                    }

                    finalCategory = subCategory;
                }

                /* ===========================
                   PRODUCT
                   =========================== */
                ProductBL product = new ProductBL(
                    item.id,
                    item.name,
                    "Description...",
                    new string[] { item.imageUrl },
                    (double)item.price,
                    20,
                    finalCategory
                );

                /* ===========================
                   TAGS
                   =========================== */
                product.tags.InsertAtEnd(item.category);

                if (!string.IsNullOrEmpty(item.subCategory))
                    product.tags.InsertAtEnd(item.subCategory);

                if (item.isTrending)
                    product.tags.InsertAtEnd("Trending");

                foreach (var c in item.colors)
                    product.tags.InsertAtEnd(c);

                /* ===========================
                   STORE
                   =========================== */
                MasterProductList.InsertAtEnd(product);
                finalCategory.products.InsertAtEnd(product);
                PriceTree.Insert(product, PriceComparator);
            }
        }

        /* ===========================
           Helpers
           =========================== */
        private CategoryBL FindCategoryByName(string name)
        {
            foreach (var cat in MasterCategoryList)
                if (cat.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return cat;

            return null;
        }

        private CategoryBL FindSubCategory(CategoryBL parent, string name)
        {
            foreach (var sub in parent.subcategories)
                if (sub.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return sub;

            return null;
        }

        /* ===========================
           DTO (ONLY for JSON)
           =========================== */
        private class ProductDto
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
