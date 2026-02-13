using System.Net.Http.Json;
using NovellaMart.Core.BL.Data_Structures;
using NovellaMart.Core.BL.Model_Classes;

namespace NovellaMart.Core.BL.Services
{
    public class ProductCatalogService
    {
        private readonly HttpClient _http;

        // REPLACED: _isInitialized boolean with a Task to handle concurrent loading
        private Task _loadingTask;

        // Custom Data Structures
        public MyLinkedList<ProductBL> MasterProductList { get; private set; }
        public MyLinkedList<CategoryBL> MasterCategoryList { get; private set; }
        public AVLTreeGeneric<ProductBL> PriceTree { get; private set; }
        public AVLTreeGeneric<ProductBL> NameTree { get; private set; }

        public ProductCatalogService(HttpClient http)
        {
            _http = http;
            MasterProductList = new MyLinkedList<ProductBL>();
            MasterCategoryList = new MyLinkedList<CategoryBL>();
            PriceTree = new AVLTreeGeneric<ProductBL>();
            NameTree = new AVLTreeGeneric<ProductBL>();
        }

        public int PriceComparator(ProductBL a, ProductBL b)
        {
            int priceComp = a.price.CompareTo(b.price);
            if (priceComp != 0) return priceComp;
            return a.product_id.CompareTo(b.product_id);
        }

        public int NameComparator(ProductBL a, ProductBL b)
        {
            return string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase);
        }

        public async Task LoadProductsAsync()
        {
            if (_loadingTask != null)
            {
                await _loadingTask;
                return;
            }

            _loadingTask = LoadDataInternalAsync();

            await _loadingTask;
        }

        private async Task LoadDataInternalAsync()
        {
            try
            {
                if (MasterProductList.Count() > 0) return;

                var rawData = await _http.GetFromJsonAsync<List<ProductDto>>("sample-data/products.json");

                if (rawData == null) return;

                foreach (var item in rawData)
                {
                    ProductBL product = new ProductBL
                    {
                        product_id = item.product_id,
                        name = item.name,
                        description = item.description,
                        images = item.images != null ? item.images.ToArray() : new string[0],
                        price = item.price,
                        stock = item.stock,
                        category = item.category
                    };

                    if (item.tags != null)
                    {
                        foreach (var t in item.tags) product.tags.InsertAtEnd(t);
                    }

                    MasterProductList.InsertAtEnd(product);
                    PriceTree.Insert(product, PriceComparator);
                    NameTree.Insert(product, NameComparator);

                    if (item.category != null)
                    {
                        var existingCat = FindCategoryById(item.category.category_id);
                        if (existingCat == null)
                        {
                            MasterCategoryList.InsertAtEnd(item.category);
                            existingCat = item.category;
                        }
                        existingCat.products.InsertAtEnd(product);
                    }
                }
            }
            catch (Exception ex)
            {
                _loadingTask = null;
                Console.WriteLine($"Error loading products: {ex.Message}");
                throw;
            }
        }

        private CategoryBL FindCategoryById(int id)
        {
            var node = MasterCategoryList.head;
            while (node != null)
            {
                if (node.Data.category_id == id) return node.Data;
                node = node.Next;
            }
            return null;
        }

        public ProductBL FindProductById(int id)
        {
            var node = MasterProductList.head;
            while (node != null)
            {
                if (node.Data.product_id == id) return node.Data;
                node = node.Next;
            }
            return null;
        }

        private class ProductDto
        {
            public int product_id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public List<string> images { get; set; }
            public double price { get; set; }
            public int stock { get; set; }
            public CategoryBL category { get; set; }
            public List<string> tags { get; set; }
        }
    }
}