using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrafficFineApp.Data;
using trafficFineManager.Services.Abstraction;
using trafficFineManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace trafficFineManager.Controllers
{
    [Authorize]
    public class TrafficFineController : Controller
    {
        private readonly ITrafficFineService _trafficFineService;
        private readonly AppDbContext _context;

        public TrafficFineController(ITrafficFineService trafficFineService, AppDbContext context)
        {
            _trafficFineService = trafficFineService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var fines = await _trafficFineService.GetAllFinesAsync();
            return View(fines);
        }

        [HttpGet]
        [Authorize(Roles = "Yonetici")]
        public async Task<IActionResult> AuditLog()
        {
            var logs = await _trafficFineService.GetAllHistoryAsync();
            return View(logs);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateViewBagsAsync();
            return View(new CreateTrafficFineViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrafficFineViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBagsAsync();
                return View(model);
            }

            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _trafficFineService.CreateFineAsync(model, currentUserId);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> History(int id)
        {
            var historyRecords = await _trafficFineService.GetFineHistoryAsync(id);
            ViewBag.TrafficFineId = id;
            return View(historyRecords);
        }

        [HttpPost]
        [Authorize(Roles = "Yonetici, Finans")]
        public async Task<IActionResult> Approve(int id)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _trafficFineService.ApproveFineAsync(id, currentUserId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Yonetici, Finans")]
        public async Task<IActionResult> Reject(RejectFineViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _trafficFineService.RejectFineAsync(model, currentUserId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var fine = await _context.TrafficFines
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(t => t.Id == id);
            
            if (fine == null) return NotFound();

            var model = new EditTrafficFineViewModel
            {
                Id = fine.Id,
                VehicleId = fine.VehicleId,
                PlateNumber = fine.Vehicle.PlateNumber,
                BrandId = fine.Vehicle.BrandId,
                ModelId = fine.Vehicle.ModelId,
                VehicleType = fine.Vehicle.VehicleType,
                OwnerName = fine.Vehicle.OwnerName,
                OwnerTC = fine.Vehicle.OwnerTC,
                FineTypeId = fine.FineTypeId,
                ViolatorName = fine.ViolatorName,
                ViolatorTC = fine.ViolatorTC,
                ViolationDate = fine.ViolationDate,
                CityId = fine.CityId,
                DistrictId = fine.DistrictId,
                ReceiptNumber = fine.ReceiptNumber 
            };

            await PopulateViewBagsAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTrafficFineViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBagsAsync();
                return View(model);
            }
            
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _trafficFineService.UpdateFineAsync(model, currentUserId);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateViewBagsAsync()
        {
            var brands = await _context.Brands.ToListAsync();
            ViewBag.Brands = brands.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToList();

            var models = await _context.Models.ToListAsync();
            ViewBag.Models = models.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();

            var vehicleTypes = Enum.GetValues(typeof(trafficFineManager.Entities.Enums.VehicleType)).Cast<trafficFineManager.Entities.Enums.VehicleType>();
            ViewBag.VehicleTypes = vehicleTypes.Select(e => new SelectListItem { Value = ((int)e).ToString(), Text = e.ToString() }).ToList();

            var fineTypes = await _context.FineTypes.Where(f => f.IsActive).ToListAsync();
            ViewBag.FineTypes = fineTypes.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = $"{f.ArticleNumber} - {f.Description} ({f.Amount}₺)"
            }).ToList();

            var cities = await _context.Cities.ToListAsync();
            ViewBag.Cities = cities.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

            var districts = await _context.Districts.ToListAsync();
            ViewBag.Districts = districts.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name }).ToList();
        }
    }
}
