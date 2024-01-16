using Customer_Web_App.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Customer_Web_App.Services.Products
{
    public class ProductService : IProductsService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private string _accessToken;
        private DateTime _tokenExpiration;

        public ProductService(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            InitializeHttpClient();
        }

        private void InitializeHttpClient()
        {
            var baseUrl = _configuration["Services:Products:BaseAddress"] ?? "";

            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("BaseAddress is missing or empty in configuration.");
            }

            _client.BaseAddress = new Uri(baseUrl);
            _client.Timeout = TimeSpan.FromSeconds(5);
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Call a method to set the authorization header with the access token
            SetAuthorizationHeader().Wait();
        }
        private bool IsTokenValid()
        {
            // Check if a token is available and not expired
            return !string.IsNullOrEmpty(_accessToken) && _tokenExpiration > DateTime.UtcNow;
        }

        private async Task SetAuthorizationHeader()
        {
            // Check if a valid token is already available
            if (!IsTokenValid())
            {
                // If not, obtain a new access token
                _accessToken = await GetAccessToken();
                _tokenExpiration = DateTime.UtcNow.AddMinutes(30); // Assuming a 30-minute expiration time
            }

            // Set the Authorization header with the obtained or cached access token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        private async Task<string> GetAccessToken()
        {
            var clientId = _configuration["Auth:ClientId"];
            var clientSecret = _configuration["Auth:ClientSecret"];
            var audience = _configuration["Services:Products:AuthAudience"];
            var domain = _configuration["Auth:Domain"];

            using (var client = new HttpClient())
            {
                var tokenEndpoint = $"https://{domain}/oauth/token";

                var tokenRequest = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "audience", audience }
                };

                var response = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(tokenRequest));
                response.EnsureSuccessStatusCode();

                var tokenResponse = await response.Content.ReadAsAsync<AccessTokenResponse>();
                return tokenResponse.AccessToken;
            }
        }

        public async Task<IEnumerable<ProductViewModel>> GetProductsAsync()
        {
            var uri = "api/products/UnderCutters";
            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAsAsync<IEnumerable<ProductViewModel>>();
            return products;
        }

        public async Task<IEnumerable<ProductViewModel>> GetProductsByNameAsync(string searchTerm)
        {
            var uri = $"api/products/UCSearch?searchTerm={searchTerm}";
            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAsAsync<IEnumerable<ProductViewModel>>();
            return products;
        }

        // Define a class to deserialize the token response
        private class AccessTokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            // Add other properties as needed (e.g., token type, expiration)
        }
    }
}
