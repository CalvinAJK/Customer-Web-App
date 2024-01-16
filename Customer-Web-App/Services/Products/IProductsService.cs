using Customer_Web_App.Models;

namespace Customer_Web_App.Services.Products
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductViewModel>> GetProductsAsync();

        Task<IEnumerable<ProductViewModel>> GetProductsByNameAsync(string searchTerm);
    }
}
