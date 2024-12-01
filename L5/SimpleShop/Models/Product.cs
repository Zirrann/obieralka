namespace SimpleShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public int StockId { get; set; }
        public Stock Stock { get; set; } // Relacja 1:1

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}