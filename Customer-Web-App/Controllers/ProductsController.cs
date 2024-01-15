using Customer_Web_App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Customer_Web_App.Controllers
{
    public class ProductsController : Controller
    {
        private HttpClient Client { get; }
        record ProductDTO(int Id, string Name, string Description, decimal Price, bool Stock);

        public ProductsController(HttpClient client,
                              IConfiguration configuration)
        {
            // FIXME: don't use HttpClient directly in controllers
            // FIXME: use a Service Proxy / Remote Facade pattern -- see "Isolation"
            var baseUrl = configuration["WebServices:Products:BaseURL"];
            client.BaseAddress = new System.Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            Client = client;
        }
        public async Task<IActionResult> Index()
        {
            var response = await Client.GetAsync("/api/products");
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAsAsync<IEnumerable<ProductDTO>>();
            var vm = products.Select(c => new ProductViewModel(
                id: c.Id,
                name: c.Name,
                description: c.Description,
                price: c.Price,
                stock: c.Stock
            ));
            return View(vm);
        }
    }
}