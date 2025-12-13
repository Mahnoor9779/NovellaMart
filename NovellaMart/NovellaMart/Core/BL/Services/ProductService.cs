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

            products.InsertAtEnd(new ProductBL(
                101, "SilkSculpt Serum", "Deep hydration for radiant skin. Deep hydration for radiant skin. Deep hydration for radiant skin. Deep hydration for radiant skin. Deep hydration for radiant skin. ",
                new[] { "https://placehold.co/400", "https://placehold.co/401" },
                3500, 20, catSkin
            ));

            products.InsertAtEnd(new ProductBL(
                102, "Argan Glow Oil", "Nourish and strengthen hair",
                new[] { "https://placehold.co/402", "https://placehold.co/403" },
                4200, 15, catHair
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
    }
}
