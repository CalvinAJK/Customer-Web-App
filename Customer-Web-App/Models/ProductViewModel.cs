namespace Customer_Web_App.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string? Name{ get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set;}
        public int? Stock { get; set;}
    }
}
