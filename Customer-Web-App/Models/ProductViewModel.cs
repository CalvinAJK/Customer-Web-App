namespace Customer_Web_App.Models
{
    public class ProductViewModel
    {
        public ProductViewModel(int id, string? name, string? description, decimal? price, bool stock)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
        }

        public int Id { get; set; }
        public string? Name{ get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set;}
        public bool Stock { get; set;}
    }
}
