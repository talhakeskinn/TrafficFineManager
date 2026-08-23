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

        public IActionResult Index()
        {
            return RedirectToAction(nameof(List), new { filter = "all" });
        }

        [HttpGet]
        [Authorize(Roles = "Yonetici, Finansman")]
        public async Task<IActionResult> List(string filter = "all")
        {
            var fines = await _trafficFineService.GetAllFinesAsync();
            
            ViewBag.CurrentFilter = filter;
            ViewBag.ShowSidebar = true;

            switch (filter)
            {
                case "yonetici_bekleyen":
                    fines = fines.Where(f => f.Status == trafficFineManager.Entities.Enums.FineStatus.Yeni || f.Status == trafficFineManager.Entities.Enums.FineStatus.YoneticiOnayinda).ToList();
                    ViewBag.FilterTitle = "Yönetici Onayı Bekleyen Cezalar";
                    break;
                case "finans_bekleyen":
                    fines = fines.Where(f => f.Status == trafficFineManager.Entities.Enums.FineStatus.FinansOnayinda).ToList();
                    ViewBag.FilterTitle = "Finans Onayı Bekleyen Cezalar";
                    break;
                case "reddedilen":
                    fines = fines.Where(f => f.Status == trafficFineManager.Entities.Enums.FineStatus.Reddedildi).ToList();
                    ViewBag.FilterTitle = "Reddedilen / İptal Edilen Cezalar";
                    break;
                case "onaylanan":
                    fines = fines.Where(f => f.Status == trafficFineManager.Entities.Enums.FineStatus.Tamamlandi).ToList();
                    ViewBag.FilterTitle = "Onaylanan ve Kesinleşen Cezalar";
                    break;
                default:
                    ViewBag.FilterTitle = "Bütün Kayıt Detayları";
                    break;
            }
            
            return View("Index", fines);
        }

        [HttpGet]
        [Authorize(Roles = "Yonetici, Finansman")]
        public async Task<IActionResult> MyApprovals()
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var allFines = await _trafficFineService.GetAllFinesAsync();
            
            var myHistoryFines = allFines.Where(f => f.Histories.Any(h => h.UserId == currentUserId && 
                (h.ActionType == trafficFineManager.Entities.Enums.ActionType.Onaylandi || 
                 h.ActionType == trafficFineManager.Entities.Enums.ActionType.Reddedildi)))
                .ToList();

            ViewBag.FilterTitle = "Onay Geçmişim";
            return View("Index", myHistoryFines);
        }

        [HttpGet]
        [Authorize(Roles = "Yonetici,Memur")]
        public async Task<IActionResult> MyCreatedFines()
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var myFines = await _trafficFineService.GetFinesByUserIdAsync(currentUserId);
            
            ViewBag.FilterTitle = "Oluşturduğum Kayıtlar";
            return View("Index", myFines);
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
            var nextReceipt = await _trafficFineService.GenerateNextReceiptNumberAsync();
            return View(new CreateTrafficFineViewModel { ReceiptNumber = nextReceipt });
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
        [Authorize(Roles = "Yonetici, Finansman")]
        public async Task<IActionResult> Approve(int id)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _trafficFineService.ApproveFineAsync(id, currentUserId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Yonetici, Finansman")]
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
        [Authorize(Roles = "Yonetici,Memur")]
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
        [Authorize(Roles = "Yonetici,Memur")]
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

        [HttpGet]
        public async Task<IActionResult> GetModelsByBrand(int brandId)
        {
            var models = await _context.Models
                .Where(m => m.BrandId == brandId)
                .Select(m => new { value = m.Id, text = m.Name })
                .ToListAsync();
            return Json(models);
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleByPlate(string plate)
        {
            if (string.IsNullOrWhiteSpace(plate)) return NotFound();
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.PlateNumber.ToUpper() == plate.ToUpper());
            
            if (vehicle == null) return NotFound();
            
            return Json(new {
                brandId = vehicle.BrandId,
                modelId = vehicle.ModelId,
                vehicleType = (int)vehicle.VehicleType,
                ownerName = vehicle.OwnerName,
                ownerTC = vehicle.OwnerTC
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetDistrictsByCity(int cityId)
        {
            var districts = await _context.Districts
                .Where(d => d.CityId == cityId)
                .Select(d => new { value = d.Id, text = d.Name })
                .ToListAsync();
            return Json(districts);
        }

        private async Task PopulateViewBagsAsync()
        {
            var brands = await _context.Brands.ToListAsync();
            ViewBag.Brands = brands.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToList();

            // Sadece seçili bir marka varsa modellerini yükleriz, aksi takdirde boş liste
            ViewBag.Models = new List<SelectListItem>();

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

            // District'leri (İlçeleri) sayfa ilk açıldığında BOMBOŞ gönderiyoruz. JavaScript ile dolacak.
            ViewBag.Districts = new List<SelectListItem>();
        }
    }
}
