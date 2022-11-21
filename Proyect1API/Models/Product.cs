namespace Proyect1API.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }

    }
}
