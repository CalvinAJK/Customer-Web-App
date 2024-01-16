using Customer_Web_App.Models;

namespace Customer_Web_App.Services.Products
{
    public class FakeProductService : IProductsService
    {
        private readonly ProductViewModel[] _products =
        {
            new ProductViewModel { Name = "Fake product E", Description = "Fake Description E", Price = 1111, Stock = true},
            new ProductViewModel { Name = "Fake product F", Description = "Fake Description F", Price = 2222, Stock = false},
            new ProductViewModel { Name = "Fake product G", Description = "Fake Description G", Price = 3333, Stock = true},
        };

        public Task<IEnumerable<ProductViewModel>> GetProductsAsync()
        {
            var products = _products.AsEnumerable();
            return Task.FromResult(products);
        }

        public Task<IEnumerable<ProductViewModel>> GetProductsByNameAsync(string searchTerm)
        {
            var filteredProducts = _products
                .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .AsEnumerable();

            return Task.FromResult(filteredProducts);
        }
    }
}
