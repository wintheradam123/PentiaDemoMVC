using Microsoft.Extensions.Caching.Memory;
using PentiaDemoMVC.Models;
using System.Text.Json;

namespace PentiaDemoMVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private const string SalespeopleCacheKey = "SalespeopleCacheKey";
        private const string OrdersCacheKey = "OrdersCacheKey";

        public ApiService(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        public async Task<List<SalesPerson>> GetSalespeopleAsync()
        {
            if (!_memoryCache.TryGetValue(SalespeopleCacheKey, out List<SalesPerson> salespeople))
            {
                var response = await _httpClient.GetAsync("SalesPersons");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                salespeople = JsonSerializer.Deserialize<List<SalesPerson>>(json, options);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                _memoryCache.Set(SalespeopleCacheKey, salespeople, cacheEntryOptions);
            }

            return salespeople;
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            if (!_memoryCache.TryGetValue(OrdersCacheKey, out List<Order> orders))
            {
                var response = await _httpClient.GetAsync("Orderlines");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                orders = JsonSerializer.Deserialize<List<Order>>(json);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                _memoryCache.Set(OrdersCacheKey, orders, cacheEntryOptions);
            }

            return orders;
        }
    }
}
