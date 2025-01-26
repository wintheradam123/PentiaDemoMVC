using Microsoft.AspNetCore.Mvc;
using PentiaDemoMVC.Services;

namespace PentiaDemoMVC.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApiService _apiService;

        public SalesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var salespeople = await _apiService.GetSalespeopleAsync();
            return View(salespeople);
        }

        public async Task<IActionResult> Details(int id)
        {
            var salespeople = await _apiService.GetSalespeopleAsync();
            var orders = await _apiService.GetOrdersAsync();

            var salesperson = salespeople.FirstOrDefault(s => s.Id == id);
            if (salesperson == null) return NotFound();

            var salespersonOrders = orders.Where(o => o.SalespersonId == id).ToList();

            ViewBag.Orders = salespersonOrders;
            return View(salesperson);
        }
    }
}
