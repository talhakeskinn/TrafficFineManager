using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TrafficFineApp.Data;
using trafficFineManager.Models;

namespace trafficFineManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> QueryPlate(string plate)
        {
            if (string.IsNullOrWhiteSpace(plate))
                return RedirectToAction(nameof(Index));

            plate = plate.Trim().ToUpper();
            
            var fines = await _context.TrafficFines
                .Include(t => t.FineType)
                .Include(t => t.Vehicle).ThenInclude(v => v.Brand)
                .Include(t => t.Vehicle).ThenInclude(v => v.Model)
                .Include(t => t.Histories)
                .Include(t => t.City)
                .Include(t => t.District)
                .Where(t => t.Vehicle.PlateNumber == plate)
                .OrderByDescending(t => t.ViolationDate)
                .ToListAsync();

            ViewBag.QueriedPlate = plate;
            ViewBag.Fines = fines;
            if (fines.Any())
            {
                var vehicle = fines.First().Vehicle;
                ViewBag.VehicleInfo = $"{vehicle.Brand.Name} {vehicle.Model.Name} ({vehicle.VehicleType})";
            }
            
            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
