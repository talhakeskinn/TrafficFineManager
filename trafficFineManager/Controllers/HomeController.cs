using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
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

        public async Task<IActionResult> Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var fines = await _context.TrafficFines.ToListAsync();
                int currentUserId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);

                var vm = new DashboardViewModel();
                var today = DateTime.Today;

                if (User.IsInRole("Yonetici"))
                {
                    vm.UserRole = "Yonetici";
                    var pendings = fines.Where(f => f.Status == trafficFineManager.Entities.Enums.FineStatus.Yeni || f.Status == trafficFineManager.Entities.Enums.FineStatus.YoneticiOnayinda).ToList();
                    vm.PendingCount = pendings.Count;
                    vm.PendingTotalAmount = pendings.Sum(x => x.Amount);
                }
                else if (User.IsInRole("Finansman"))
                {
                    vm.UserRole = "Finansman";
                    var pendings = fines.Where(f => f.Status == trafficFineManager.Entities.Enums.FineStatus.FinansOnayinda).ToList();
                    vm.PendingCount = pendings.Count;
                    vm.PendingTotalAmount = pendings.Sum(x => x.Amount);
                }
                else
                {
                    vm.UserRole = "Memur";
                    var pendings = fines.Where(f => f.CreatorUserId == currentUserId && (f.Status != trafficFineManager.Entities.Enums.FineStatus.Tamamlandi && f.Status != trafficFineManager.Entities.Enums.FineStatus.Reddedildi)).ToList();
                    vm.PendingCount = pendings.Count;
                    vm.PendingTotalAmount = pendings.Sum(x => x.Amount);
                    
                    // Filter fines to only this user's fines for the rest of the stats
                    fines = fines.Where(f => f.CreatorUserId == currentUserId).ToList();
                }

                // Genel İstatistikler
                vm.TotalCount = fines.Count;
                var approvedFines = fines.Where(f => f.Status == trafficFineManager.Entities.Enums.FineStatus.Tamamlandi).ToList();
                vm.ApprovedCount = approvedFines.Count;
                vm.ApprovedTotalAmount = approvedFines.Sum(f => f.Amount);
                vm.RejectedCount = fines.Count(f => f.Status == trafficFineManager.Entities.Enums.FineStatus.Reddedildi);

                // Zaman Bazlı İstatistikler (Oluşturulma tarihine göre)
                var dailyFines = fines.Where(f => f.CreatedAt.Date == today).ToList();
                vm.DailyCount = dailyFines.Count;
                vm.DailyTotal = dailyFines.Sum(f => f.Amount);

                // Haftalık (Son 7 Gün)
                var weeklyFines = fines.Where(f => f.CreatedAt.Date >= today.AddDays(-7)).ToList();
                vm.WeeklyCount = weeklyFines.Count;
                vm.WeeklyTotal = weeklyFines.Sum(f => f.Amount);

                // Aylık (Bu ay)
                var monthlyFines = fines.Where(f => f.CreatedAt.Month == today.Month && f.CreatedAt.Year == today.Year).ToList();
                vm.MonthlyCount = monthlyFines.Count;
                vm.MonthlyTotal = monthlyFines.Sum(f => f.Amount);

                // Yıllık (Bu yıl)
                var yearlyFines = fines.Where(f => f.CreatedAt.Year == today.Year).ToList();
                vm.YearlyCount = yearlyFines.Count;
                vm.YearlyTotal = yearlyFines.Sum(f => f.Amount);

                // En Fazla Ceza Yiyen Kişi
                if (fines.Any())
                {
                    var mostFined = fines.GroupBy(f => f.ViolatorTC)
                                         .Select(g => new { TC = g.Key, Name = g.First().ViolatorName, Count = g.Count(), Total = g.Sum(x => x.Amount) })
                                         .OrderByDescending(g => g.Count)
                                         .FirstOrDefault();
                                         
                    if (mostFined != null)
                    {
                        vm.MostFinedPersonTC = mostFined.TC;
                        vm.MostFinedPersonName = mostFined.Name;
                        vm.MostFinedPersonCount = mostFined.Count;
                        vm.MostFinedPersonTotalAmount = mostFined.Total;
                    }
                }

                return View("Dashboard", vm);
            }
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
