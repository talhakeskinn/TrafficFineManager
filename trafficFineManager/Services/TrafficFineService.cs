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
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.PlateNumber == model.PlateNumber);

            if (vehicle == null)
            {
                vehicle = new Vehicle
                {
                    PlateNumber = model.PlateNumber,
                    BrandId = model.BrandId,
                    ModelId = model.ModelId,
                    VehicleType = model.VehicleType,
                    OwnerName = model.OwnerName,
                    OwnerTC = model.OwnerTC,
                    IsActive = true
                };

                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
            }

            var fineType = await _context.FineTypes.FindAsync(model.FineTypeId);
            if (fineType == null) throw new Exception("Ceza maddesi bulunamadı.");

            var fine = new TrafficFine
            {
                VehicleId = vehicle.Id,
                FineTypeId = model.FineTypeId,
                ViolatorName = model.ViolatorName,
                ViolatorTC = model.ViolatorTC,
                CityId = model.CityId,
                DistrictId = model.DistrictId,
                ViolationReason = fineType.Description, 
                Amount = fineType.Amount,               
                ReceiptNumber = model.ReceiptNumber, 
                CreatorUserId = creatorUserId,
                Status = FineStatus.Yeni,
                ViolationDate = model.ViolationDate,
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
                Description = "Ceza sisteme eklendi."
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
                .Include(t => t.City)
                .Include(t => t.District)
                .Include(t => t.Histories)
                .OrderByDescending(t => t.NotificationDate)
                .ToListAsync();
        }

        public async Task<List<TrafficFine>> GetFinesByUserIdAsync(int userId)
        {
            return await _context.TrafficFines
                .Include(t => t.CreatorUser)
                .Include(t => t.FineType)
                .Include(t => t.Vehicle)
                    .ThenInclude(v => v.Brand) 
                .Include(t => t.Vehicle)
                    .ThenInclude(v => v.Model) 
                .Include(t => t.City)
                .Include(t => t.District)
                .Include(t => t.Histories)
                .Where(t => t.CreatorUserId == userId)
                .OrderByDescending(t => t.NotificationDate)
                .ToListAsync();
        }
        public async Task ApproveFineAsync(int id, int userId)
        {
            var fine = await _context.TrafficFines.FindAsync(id);
            if (fine == null || fine.Status == FineStatus.Tamamlandi || fine.Status == FineStatus.Reddedildi) return;

            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return;

            var oldStatus = fine.Status;
            string description = "";

            if (user.Role.Name == "Yonetici" && (fine.Status == FineStatus.Yeni || fine.Status == FineStatus.YoneticiOnayinda))
            {
                fine.Status = FineStatus.FinansOnayinda;
                description = "Yönetici onayı verildi. Finans onayı bekleniyor.";
            }
            else if (user.Role.Name == "Finansman" && fine.Status == FineStatus.FinansOnayinda)
            {
                fine.Status = FineStatus.Tamamlandi;
                description = "Finans onayı verildi. İşlem kesinleşti (Tamamlandı).";
            }
            else
            {
                // Unhandled role or state mismatch
                return;
            }

            var history = new TrafficFineHistory
            {
                TrafficFineId = fine.Id,
                UserId = userId,
                ActionType = ActionType.Onaylandi,
                OldStatus = oldStatus,
                NewStatus = fine.Status,
                Description = description
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
                NewStatus = FineStatus.Reddedildi,
                Description = string.IsNullOrWhiteSpace(model.RejectReason) ? "Ceza iptal edildi." : model.RejectReason
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
            
            if (fine == null) throw new Exception("Ceza bulunamadı");

            fine.Vehicle.BrandId = model.BrandId;
            fine.Vehicle.ModelId = model.ModelId;
            fine.Vehicle.VehicleType = model.VehicleType;
            fine.Vehicle.OwnerName = model.OwnerName;
            fine.Vehicle.OwnerTC = model.OwnerTC;

            var fineType = await _context.FineTypes.FindAsync(model.FineTypeId);
            if (fineType == null) throw new Exception("Ceza maddesi bulunamadı.");

            fine.FineTypeId = model.FineTypeId;
            fine.ViolatorName = model.ViolatorName;
            fine.ViolatorTC = model.ViolatorTC;
            fine.CityId = model.CityId;
            fine.DistrictId = model.DistrictId;
            fine.ViolationReason = fineType.Description;
            fine.Amount = fineType.Amount;
            fine.ViolationDate = model.ViolationDate;
            fine.ReceiptNumber = model.ReceiptNumber;

            var history = new TrafficFineHistory
            {
                TrafficFineId = fine.Id,
                UserId = userId,
                ActionType = ActionType.Guncellendi,
                OldStatus = fine.Status,
                NewStatus = fine.Status, 
                Description = "Ceza ve/veya araç bilgileri güncellendi."
            };

            _context.TrafficFineHistories.Add(history);
            await _context.SaveChangesAsync();
        }
    }
}