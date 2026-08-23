using Microsoft.EntityFrameworkCore;
using TrafficFineApp.Data;
using trafficFineManager.Entities;
using trafficFineManager.Entities.Enums;
using trafficFineManager.Services.Abstraction;
using trafficFineManager.ViewModels;

namespace trafficFineManager.Services
{
    public class TrafficFineService : ITrafficFineService
    {
        private readonly AppDbContext _context;

        public TrafficFineService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateFineAsync(CreateTrafficFineViewModel model, int creatorUserId)
        {
            var fineType = await _context.FineTypes.FindAsync(model.FineTypeId);
            if (fineType == null) throw new Exception("Geçersiz ceza maddesi.");

            // 1. Aracı Plakadan Bul (Yoksa Yeni Kayıt Oluştur)
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.PlateNumber == model.PlateNumber.Trim().ToUpper());
            if (vehicle == null)
            {
                vehicle = new Vehicle
                {
                    PlateNumber = model.PlateNumber.Trim().ToUpper(),
                    BrandId = model.BrandId,
                    ModelId = model.ModelId,
                    VehicleType = model.VehicleType,
                    OwnerName = model.OwnerName,
                    OwnerTC = model.OwnerTC,
                    IsActive = true
                };
                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync(); // ID almak için kaydediyoruz
            }

            // 2. Cezayı Oluştur
            var fine = new TrafficFine
            {
                VehicleId = vehicle.Id,
                FineTypeId = model.FineTypeId,
                ViolatorName = model.ViolatorName,
                ViolatorTC = model.ViolatorTC,
                ViolationReason = fineType.Description, 
                Amount = fineType.Amount,               
                ReceiptNumber = model.ReceiptNumber, 
                CreatorUserId = creatorUserId,
                Status = FineStatus.Yeni,
                NotificationDate = DateTime.Now
            };

            _context.TrafficFines.Add(fine);

            await _context.SaveChangesAsync();

            var history = new TrafficFineHistory
            {
                TrafficFineId = fine.Id,
                UserId = creatorUserId,
                ActionType = ActionType.Olusturuldu,
                NewStatus = FineStatus.Yeni,
                Description = $"Ceza sisteme oluşturuldu. Madde: {fineType.ArticleNumber}"
            };

            _context.TrafficFineHistories.Add(history);
            await _context.SaveChangesAsync(); 
        }
        public async Task<List<TrafficFine>> GetAllFinesAsync()
        {
            return await _context.TrafficFines
                .Include(t => t.CreatorUser)
                .Include(t => t.FineType)
                .Include(t => t.Vehicle)
                    .ThenInclude(v => v.Brand) 
                .Include(t => t.Vehicle)
                    .ThenInclude(v => v.Model) 
                .OrderByDescending(t => t.NotificationDate)
                .ToListAsync();
        }
        public async Task ApproveFineAsync(int id, int userId)
        {
            var fine = await _context.TrafficFines.FindAsync(id);
            if (fine == null || fine.Status == FineStatus.Tamamlandi || fine.Status == FineStatus.Reddedildi) return;

            var oldStatus = fine.Status;

            // Durum Makinesi (State Machine): Bir sonraki aşamaya geçir
            if (fine.Status == FineStatus.Yeni) fine.Status = FineStatus.YoneticiOnayinda;
            else if (fine.Status == FineStatus.YoneticiOnayinda) fine.Status = FineStatus.FinansOnayinda;
            else if (fine.Status == FineStatus.FinansOnayinda) fine.Status = FineStatus.Tamamlandi;

            var history = new TrafficFineHistory
            {
                TrafficFineId = fine.Id,
                UserId = userId,
                ActionType = ActionType.Onaylandi,
                OldStatus = oldStatus,
                NewStatus = fine.Status,
                Description = "Ceza onaylandı ve bir sonraki aşamaya aktarıldı."
            };

            _context.TrafficFineHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task RejectFineAsync(RejectFineViewModel model, int userId)
        {
            var fine = await _context.TrafficFines.FindAsync(model.TrafficFineId);
            if (fine == null || fine.Status == FineStatus.Tamamlandi || fine.Status == FineStatus.Reddedildi) return;

            var oldStatus = fine.Status;
            fine.Status = FineStatus.Reddedildi;

            var history = new TrafficFineHistory
            {
                TrafficFineId = fine.Id,
                UserId = userId,
                ActionType = ActionType.Reddedildi,
                OldStatus = oldStatus,
                NewStatus = fine.Status,
                Description = $"Ceza reddedildi. Neden: {model.RejectReason}"
            };

            _context.TrafficFineHistories.Add(history);
            await _context.SaveChangesAsync();
        }
        public async Task<List<TrafficFineHistory>> GetFineHistoryAsync(int trafficFineId)
        {
            return await _context.TrafficFineHistories
                .Include(h => h.User).ThenInclude(u => u.Role)
                .Where(h => h.TrafficFineId == trafficFineId)
                .OrderByDescending(h => h.Id) 
                .ToListAsync();
        }

        public async Task<List<TrafficFineHistory>> GetAllHistoryAsync()
        {
            return await _context.TrafficFineHistories
                .Include(h => h.User).ThenInclude(u => u.Role)
                .Include(h => h.TrafficFine).ThenInclude(t => t.Vehicle)
                .OrderByDescending(h => h.Id) 
                .ToListAsync();
        }
        public async Task UpdateFineAsync(EditTrafficFineViewModel model, int userId)
        {
            var fine = await _context.TrafficFines
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(t => t.Id == model.Id);
            
            if (fine == null) return;

            if (fine.Status == FineStatus.Tamamlandi || fine.Status == FineStatus.Reddedildi) return;

            var fineType = await _context.FineTypes.FindAsync(model.FineTypeId);
            if (fineType == null) throw new Exception("Geçersiz ceza maddesi.");

            // Araç Bilgilerini Güncelle
            fine.Vehicle.PlateNumber = model.PlateNumber.Trim().ToUpper();
            fine.Vehicle.BrandId = model.BrandId;
            fine.Vehicle.ModelId = model.ModelId;
            fine.Vehicle.VehicleType = model.VehicleType;
            fine.Vehicle.OwnerName = model.OwnerName;
            fine.Vehicle.OwnerTC = model.OwnerTC;

            // Ceza Bilgilerini Güncelle
            fine.FineTypeId = model.FineTypeId;
            fine.ViolatorName = model.ViolatorName;
            fine.ViolatorTC = model.ViolatorTC;
            fine.ViolationReason = fineType.Description;
            fine.Amount = fineType.Amount;
            fine.ReceiptNumber = model.ReceiptNumber;

            var history = new TrafficFineHistory
            {
                TrafficFineId = fine.Id,
                UserId = userId,
                ActionType = ActionType.Guncellendi,
                OldStatus = fine.Status,
                NewStatus = fine.Status, 
                Description = $"Ceza detayları güncellendi. Yeni Madde: {fineType.ArticleNumber}"
            };

            _context.TrafficFineHistories.Add(history);
            await _context.SaveChangesAsync();
        }
    }
}