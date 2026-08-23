using Microsoft.EntityFrameworkCore;
using trafficFineManager.Entities;

namespace TrafficFineApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<AppRole> Roles { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<TrafficFine> TrafficFines { get; set; }
        public DbSet<TrafficFineHistory> TrafficFineHistories { get; set; }
        public DbSet<FineType> FineTypes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TrafficFineHistory>()
                .HasOne(h => h.TrafficFine)
                .WithMany(t => t.Histories)
                .HasForeignKey(h => h.TrafficFineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrafficFineHistory>()
                .HasOne(h => h.User)
                .WithMany()
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Brand)
                .WithMany()
                .HasForeignKey(v => v.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Model)
                .WithMany()
                .HasForeignKey(v => v.ModelId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<TrafficFine>()
                .HasOne(t => t.FineType)
                .WithMany()
                .HasForeignKey(t => t.FineTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppRole>().HasData(
                new AppRole { Id = 1, Name = "Memur" },
                new AppRole { Id = 2, Name = "Yonetici" },
                new AppRole { Id = 3, Name = "Finansman" }
            );

            modelBuilder.Entity<AppUser>().HasData(
                new AppUser { Id = 1, RoleId = 1, RegistrationNumber = "S-001", IdentityNumber = "11111111111", FirstName = "Ahmet", LastName = "Memur", Email = "ahmet@test.com", PasswordHash = "123456" },
                new AppUser { Id = 2, RoleId = 2, RegistrationNumber = "S-002", IdentityNumber = "22222222222", FirstName = "Ayşe", LastName = "Yönetici", Email = "ayse@test.com", PasswordHash = "123456" },
                new AppUser { Id = 3, RoleId = 3, RegistrationNumber = "S-003", IdentityNumber = "33333333333", FirstName = "Fatma", LastName = "Finans", Email = "fatma@test.com", PasswordHash = "123456" }
            );

            modelBuilder.Entity<FineType>().HasData(
                new FineType { Id = 1, ArticleNumber = "47/1-b", Description = "Kırmızı ışık kuralına uymamak", Amount = 1506.00M, IsActive = true },
                new FineType { Id = 2, ArticleNumber = "51/2-a", Description = "Hız sınırını %10'dan %30'a kadar aşmak", Amount = 1506.00M, IsActive = true },
                new FineType { Id = 3, ArticleNumber = "51/2-b", Description = "Hız sınırını %30'dan %50'ye kadar aşmak", Amount = 3135.00M, IsActive = true },
                new FineType { Id = 4, ArticleNumber = "73/c", Description = "Seyir halinde cep telefonu kullanmak", Amount = 1506.00M, IsActive = true },
                new FineType { Id = 5, ArticleNumber = "48/5", Description = "Alkollü araç kullanmak (1. Defa)", Amount = 6439.00M, IsActive = true }
            );

            modelBuilder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "Renault" },
                new Brand { Id = 2, Name = "Ford" },
                new Brand { Id = 3, Name = "Fiat" },
                new Brand { Id = 4, Name = "Toyota" },
                new Brand { Id = 5, Name = "Volkswagen" }
            );

            modelBuilder.Entity<Model>().HasData(
                new Model { Id = 1, BrandId = 1, Name = "Clio" },
                new Model { Id = 2, BrandId = 1, Name = "Megane" },
                new Model { Id = 3, BrandId = 2, Name = "Focus" },
                new Model { Id = 4, BrandId = 2, Name = "Fiesta" },
                new Model { Id = 5, BrandId = 3, Name = "Egea" },
                new Model { Id = 6, BrandId = 4, Name = "Corolla" },
                new Model { Id = 7, BrandId = 5, Name = "Passat" }
            );

            modelBuilder.Entity<Vehicle>().HasData(
                new Vehicle { Id = 1, BrandId = 1, ModelId = 1, PlateNumber = "34ABC123", VehicleType = trafficFineManager.Entities.Enums.VehicleType.Binek, OwnerName = "Ali Yılmaz", OwnerTC = "11111111110" },
                new Vehicle { Id = 2, BrandId = 2, ModelId = 3, PlateNumber = "06XYZ987", VehicleType = trafficFineManager.Entities.Enums.VehicleType.Kiralik, OwnerName = "ABC Rent A Car", OwnerTC = "22222222220" },
                new Vehicle { Id = 3, BrandId = 3, ModelId = 5, PlateNumber = "35DEF456", VehicleType = trafficFineManager.Entities.Enums.VehicleType.Binek, OwnerName = "Mehmet Demir", OwnerTC = "33333333330" }
            );

            modelBuilder.Entity<TrafficFine>()
                .HasOne(t => t.City)
                .WithMany()
                .HasForeignKey(t => t.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrafficFine>()
                .HasOne(t => t.District)
                .WithMany()
                .HasForeignKey(t => t.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "İstanbul" },
                new City { Id = 2, Name = "Ankara" }
            );

            modelBuilder.Entity<District>().HasData(
                new District { Id = 1, CityId = 1, Name = "Kadıköy" },
                new District { Id = 2, CityId = 1, Name = "Beşiktaş" },
                new District { Id = 3, CityId = 1, Name = "Şişli" },
                new District { Id = 4, CityId = 2, Name = "Çankaya" },
                new District { Id = 5, CityId = 2, Name = "Yenimahalle" }
            );

            var currentDate = new DateTime(2026, 8, 23, 12, 0, 0);
            var pastDate1 = currentDate.AddDays(-45);
            var pastDate2 = currentDate.AddDays(-20);
            var pastDate3 = currentDate.AddDays(-5);

            modelBuilder.Entity<TrafficFine>().HasData(
                new TrafficFine 
                { 
                    Id = 1, VehicleId = 1, FineTypeId = 1, ViolatorName = "Ali Yılmaz", ViolatorTC = "11111111110", 
                    CityId = 1, DistrictId = 1, ViolationReason = "Kırmızı ışık kuralına uymamak", Amount = 1506.00M, 
                    ViolationDate = pastDate1, NotificationDate = pastDate1.AddHours(2), CreatedAt = pastDate1.AddHours(2),
                    Status = trafficFineManager.Entities.Enums.FineStatus.Tamamlandi, ReceiptNumber = "TR-2026-001", CreatorUserId = 1 
                },
                new TrafficFine 
                { 
                    Id = 2, VehicleId = 2, FineTypeId = 2, ViolatorName = "Ayşe Demir", ViolatorTC = "44444444440", 
                    CityId = 2, DistrictId = 4, ViolationReason = "Hız sınırını %10'dan %30'a kadar aşmak", Amount = 1506.00M, 
                    ViolationDate = pastDate2, NotificationDate = pastDate2.AddHours(1), CreatedAt = pastDate2.AddHours(1),
                    Status = trafficFineManager.Entities.Enums.FineStatus.Reddedildi, ReceiptNumber = "TR-2026-002", CreatorUserId = 1 
                },
                new TrafficFine 
                { 
                    Id = 3, VehicleId = 1, FineTypeId = 4, ViolatorName = "Ali Yılmaz", ViolatorTC = "11111111110", 
                    CityId = 1, DistrictId = 2, ViolationReason = "Seyir halinde cep telefonu kullanmak", Amount = 1506.00M, 
                    ViolationDate = pastDate3, NotificationDate = pastDate3.AddMinutes(45), CreatedAt = pastDate3.AddMinutes(45),
                    Status = trafficFineManager.Entities.Enums.FineStatus.FinansOnayinda, ReceiptNumber = "TR-2026-003", CreatorUserId = 1 
                },
                new TrafficFine 
                { 
                    Id = 4, VehicleId = 3, FineTypeId = 3, ViolatorName = "Mehmet Demir", ViolatorTC = "33333333330", 
                    CityId = 2, DistrictId = 5, ViolationReason = "Hız sınırını %30'dan %50'ye kadar aşmak", Amount = 3135.00M, 
                    ViolationDate = currentDate.AddHours(-2), NotificationDate = currentDate.AddHours(-1), CreatedAt = currentDate.AddHours(-1),
                    Status = trafficFineManager.Entities.Enums.FineStatus.Yeni, ReceiptNumber = "TR-2026-004", CreatorUserId = 1 
                }
            );

            modelBuilder.Entity<TrafficFineHistory>().HasData(
                new TrafficFineHistory { Id = 1, TrafficFineId = 1, UserId = 1, ActionType = trafficFineManager.Entities.Enums.ActionType.Olusturuldu, OldStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni, NewStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni, Description = "Ceza sisteme eklendi.", ActionDate = pastDate1.AddHours(2) },
                new TrafficFineHistory { Id = 2, TrafficFineId = 1, UserId = 2, ActionType = trafficFineManager.Entities.Enums.ActionType.Onaylandi, OldStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni, NewStatus = trafficFineManager.Entities.Enums.FineStatus.FinansOnayinda, Description = "Yönetici onayı verildi. Finans onayı bekleniyor.", ActionDate = pastDate1.AddDays(1) },
                new TrafficFineHistory { Id = 3, TrafficFineId = 1, UserId = 3, ActionType = trafficFineManager.Entities.Enums.ActionType.Onaylandi, OldStatus = trafficFineManager.Entities.Enums.FineStatus.FinansOnayinda, NewStatus = trafficFineManager.Entities.Enums.FineStatus.Tamamlandi, Description = "Finans onayı verildi. İşlem kesinleşti (Tamamlandı).", ActionDate = pastDate1.AddDays(2) },

                new TrafficFineHistory { Id = 4, TrafficFineId = 2, UserId = 1, ActionType = trafficFineManager.Entities.Enums.ActionType.Olusturuldu, OldStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni, NewStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni, Description = "Ceza sisteme eklendi.", ActionDate = pastDate2.AddHours(1) },
                new TrafficFineHistory { Id = 5, TrafficFineId = 2, UserId = 2, ActionType = trafficFineManager.Entities.Enums.ActionType.Reddedildi, OldStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni, NewStatus = trafficFineManager.Entities.Enums.FineStatus.Reddedildi, Description = "Plaka okunamıyor, kayıt reddedildi.", ActionDate = pastDate2.AddDays(1) },

                new TrafficFineHistory { Id = 6, TrafficFineId = 3, UserId = 1, ActionType = trafficFineManager.Entities.Enums.ActionType.Olusturuldu, OldStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni, NewStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni, Description = "Ceza sisteme eklendi.", ActionDate = pastDate3.AddMinutes(45) },
                new TrafficFineHistory { Id = 7, TrafficFineId = 3, UserId = 2, ActionType = trafficFineManager.Entities.Enums.ActionType.Onaylandi, OldStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni, NewStatus = trafficFineManager.Entities.Enums.FineStatus.FinansOnayinda, Description = "Yönetici onayı verildi. Finans onayı bekleniyor.", ActionDate = pastDate3.AddDays(1) },

                new TrafficFineHistory { Id = 8, TrafficFineId = 4, UserId = 1, ActionType = trafficFineManager.Entities.Enums.ActionType.Olusturuldu, OldStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni, NewStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni, Description = "Ceza sisteme eklendi.", ActionDate = currentDate.AddHours(-1) }
            );
        }
    }
}