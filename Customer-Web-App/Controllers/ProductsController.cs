﻿using Customer_Web_App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Customer_Web_App.Controllers
{
    public class ProductsController : Controller
    {
        private HttpClient Client { get; }
        record ProductDTO(int Id, string Name, string Description, decimal Price, int Stock);

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
            var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
            var vm = products.Select(c => new ProductViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price,
                Stock = c.Stock,
            });
            return View(vm);
        }
    }
}