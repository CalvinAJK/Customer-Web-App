using Customer_Web_App.Models;
using Customer_Web_App.Services.Products;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Customer_Web_App.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductsService _productsService; 

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productsService.GetProductsAsync();

                if (products != null)
                {
                    return View(products);
                }
                else
                {
                    return View("Error"); // Create an "Error" view for handling this case
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return View("Error"); // Create an "Error" view for handling exceptions
            }
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                // Handle invalid search term, maybe return a different view or show an error message
                return RedirectToAction("Index");
            }

            var products = await _productsService.GetProductsByNameAsync(searchTerm);
            return View("Index", products);
        }
    }
}