using System.Net;
using Customer_Web_App.Controllers;
using Customer_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;

namespace Customer_Web_App.Tests
{
    [TestClass]
    public class ProductsControllerTests
    {
        [TestMethod]
        public async Task Index_ReturnsViewWithProducts()
        {
            // Arrange
            var expectedProducts = new List<ProductViewModel>
            {
                new ProductViewModel(1, "Product1", "Description1", 19.99m, 50),
                new ProductViewModel(2, "Product2", "Description2", 29.99m, 30),
                // Add more sample products as needed
            };

            // Mock IConfiguration
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["WebServices:Products:BaseURL"]).Returns("http://example.com");

            // Mock HttpClient
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(expectedProducts)),
                });

            var httpClient = new HttpClient(handlerMock.Object);

            // Create the controller with mocked dependencies
            var productsController = new ProductsController(httpClient, configurationMock.Object);

            // Act
            var result = await productsController.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as IEnumerable<ProductViewModel>;
            Assert.IsNotNull(model);

            Assert.AreEqual(expectedProducts.Count + 1, model.Count());

            // You can add more specific assertions based on your actual data and expectations
            // For example, check if the product details match the expected values.
        }
    }
}
