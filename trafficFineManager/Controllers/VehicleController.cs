using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrafficFineApp.Data;
using trafficFineManager.Entities;
using trafficFineManager.Entities.Enums;
using trafficFineManager.ViewModels;

namespace trafficFineManager.Controllers
{
    [Authorize(Roles = "Yonetici,StandartKullanici")]
    public class VehicleController : Controller
    {
        private readonly AppDbContext _context;

        public VehicleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Vehicle
        public async Task<IActionResult> Index()
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Model)
                .ToListAsync();
            return View(vehicles);
        }

        // GET: Vehicle/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var vehicle = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Model)
                .Include(v => v.TrafficFines)
                    .ThenInclude(f => f.FineType)
                .Include(v => v.TrafficFines)
                    .ThenInclude(f => f.City)
                .Include(v => v.TrafficFines)
                    .ThenInclude(f => f.District)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vehicle == null) return NotFound();

            return View(vehicle);
        }

        // GET: Vehicle/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        // POST: Vehicle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlateNumber,BrandId,ModelId,VehicleType,OwnerName,OwnerTC")] Vehicle vehicle)
        {
            // Plaka zaten var mı?
            if (await _context.Vehicles.AnyAsync(v => v.PlateNumber.ToUpper() == vehicle.PlateNumber.ToUpper()))
            {
                ModelState.AddModelError("PlateNumber", "Bu plaka zaten sistemde kayıtlı!");
            }

            if (ModelState.IsValid)
            {
                vehicle.PlateNumber = vehicle.PlateNumber.ToUpper();
                vehicle.IsActive = true;
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Araç başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            
            await PopulateDropdownsAsync();
            return View(vehicle);
        }

        private async Task PopulateDropdownsAsync()
        {
            var brands = await _context.Brands.OrderBy(b => b.Name).ToListAsync();
            ViewBag.Brands = brands.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToList();
            
            // Initial empty models list (will be populated by JS)
            ViewBag.Models = new List<SelectListItem>();

            var vehicleTypes = Enum.GetValues(typeof(VehicleType)).Cast<VehicleType>();
            ViewBag.VehicleTypes = vehicleTypes.Select(e => new SelectListItem { Value = ((int)e).ToString(), Text = e.ToString() }).ToList();
        }
    }
}

