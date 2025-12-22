using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.BL.Data_Structures;

namespace NovellaMart.Core.BL.Services
{
    public class ProductService
    {
        private static MyLinkedList<ProductBL> products = new MyLinkedList<ProductBL>();

        public ProductService()
        {
            if (products.IsEmpty())
            {
                SeedProducts();
            }
        }
        private void SeedProducts()
        {
            var catSkin = new CategoryBL(1, "Skin Care", 0);
            var catHair = new CategoryBL(2, "Hair Care", 0);
            var catBody = new CategoryBL(3, "Body Care", 0);
            var catFace = new CategoryBL(4, "Face Masks", 0);

            products.InsertAtEnd(new ProductBL(
                101, "SilkSculpt Serum", "Deep hydration for radiant skin.",
                new[] { "https://placehold.co/400", "https://placehold.co/401" },
                3500, 20, catSkin
            ));

            products.InsertAtEnd(new ProductBL(
                102, "Argan Glow Oil", "Nourish and strengthen hair",
                new[] { "https://placehold.co/402", "https://placehold.co/403" },
                4200, 15, catHair
            ));

            products.InsertAtEnd(new ProductBL(
                103, "Lavender Body Lotion", "Moisturizes and softens skin",
                new[] { "https://placehold.co/404", "https://placehold.co/405" },
                1800, 10, catBody
            ));

            products.InsertAtEnd(new ProductBL(
                104, "Vitamin C Face Mask", "Brightens and revitalizes dull skin",
                new[] { "https://placehold.co/406", "https://placehold.co/407" },
                2200, 15, catFace
            ));

            products.InsertAtEnd(new ProductBL(
                105, "Coconut Hair Shampoo", "Strengthens hair roots and adds shine",
                new[] { "https://placehold.co/408", "https://placehold.co/409" },
                1500, 10, catHair
            ));

            products.InsertAtEnd(new ProductBL(
                106, "Aloe Vera Gel", "Soothes and hydrates skin naturally",
                new[] { "https://placehold.co/410", "https://placehold.co/411" },
                1200, 5, catSkin
            ));

            products.InsertAtEnd(new ProductBL(
                107, "Herbal Foot Cream", "Refreshes and softens tired feet",
                new[] { "https://placehold.co/412", "https://placehold.co/413" },
                900, 8, catBody
            ));

            products.InsertAtEnd(new ProductBL(
                108, "Charcoal Face Scrub", "Detoxifies and cleanses pores",
                new[] { "https://placehold.co/414", "https://placehold.co/415" },
                2000, 12, catFace
            ));

            products.InsertAtEnd(new ProductBL(
                109, "Rose Water Toner", "Balances skin pH and refreshes",
                new[] { "https://placehold.co/416", "https://placehold.co/417" },
                1400, 7, catSkin
            ));

            products.InsertAtEnd(new ProductBL(
                110, "Mint Hair Conditioner", "Adds shine and detangles hair",
                new[] { "https://placehold.co/418", "https://placehold.co/419" },
                1600, 10, catHair
            ));
        }


        public ProductBL GetProductById(int id)
        {
            var current = products.head;
            while (current != null)
            {
                if (current.Data.product_id == id)
                    return current.Data;

                current = current.Next;
            }
            return null;
        }

        public MyLinkedList<ProductBL> GetAllProducts()
        {
            return products;
        }

    }
}
