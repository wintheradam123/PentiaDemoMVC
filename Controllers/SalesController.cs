using Microsoft.AspNetCore.Mvc;
using PentiaDemoMVC.Services;
using System.Globalization;

namespace PentiaDemoMVC.Controllers
{
    // Primary constructor used? :O
    public class SalesController(ApiService apiService) : Controller
    {
        private readonly ApiService _apiService = apiService;

        public async Task<IActionResult> Index()
        {
            var salespeople = await _apiService.GetSalespeopleWithOrderCountAsync();
            return View(salespeople);
        }

        public async Task<IActionResult> Details(int id)
        {
            var salespeople = await _apiService.GetSalespeopleWithOrderCountAsync();
            var salesperson = salespeople.FirstOrDefault(s => s.Id == id);
            if (salesperson == null) return NotFound();

            var orders = await _apiService.GetOrdersAsync();
            var salespersonOrders = orders.Where(o => o.SalespersonId == id).ToList();

            ViewBag.Orders = salespersonOrders;
            return View(salesperson);
        }

        public async Task<IActionResult> OrderGraph()
        {
            var orders = await _apiService.GetOrdersAsync();

            var groupedOrders = orders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                    OrderCount = g.Count()
                })
                .OrderBy(g => g.Month) // TODO: Fix ordering
                .ToList();

            ViewBag.MonthlyOrders = groupedOrders;
            return View();
        }
    }
}
