using Microsoft.Extensions.Caching.Memory;
using PentiaDemoMVC.Models;
using System.Text.Json;

namespace PentiaDemoMVC.Services
{
    public class ApiService(HttpClient httpClient, IMemoryCache memoryCache, ILogger<ApiService> logger)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly ILogger<ApiService> _logger = logger;
        private const string SalespeopleCacheKey = "SalespeopleCacheKey";
        private const string OrdersCacheKey = "OrdersCacheKey";

        public async Task<List<SalesPerson>> GetSalespeopleAsync()
        {
            if (!_memoryCache.TryGetValue(SalespeopleCacheKey, out List<SalesPerson> salespeople))
            {
                try
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
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Error fetching salespeople from API.");
                    salespeople = [];
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred.");
                    salespeople = [];
                }
            }

            return salespeople;
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            if (!_memoryCache.TryGetValue(OrdersCacheKey, out List<Order> orders))
            {
                try
                {
                    var response = await _httpClient.GetAsync("Orderlines");
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new CustomDateTimeConverter() }
                    };

                    orders = JsonSerializer.Deserialize<List<Order>>(json, options);

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                    _memoryCache.Set(OrdersCacheKey, orders, cacheEntryOptions);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Error fetching orders from API.");
                    orders = [];
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred.");
                    orders = [];
                }
            }

            return orders;
        }

        public async Task<List<SalesPerson>> GetSalespeopleWithOrderCountAsync()
        {
            var salespeople = await GetSalespeopleAsync();
            var orders = await GetOrdersAsync();

            foreach (var salesperson in salespeople)
            {
                salesperson.OrderCount = orders.Count(o => o.SalespersonId == salesperson.Id);
            }

            return salespeople;
        }
    }
}
