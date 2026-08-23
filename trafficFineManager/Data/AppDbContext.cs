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

            // 1. Roller Ekleniyor
            modelBuilder.Entity<AppRole>().HasData(
                new AppRole { Id = 1, Name = "Memur" },
                new AppRole { Id = 2, Name = "Yonetici" },
                new AppRole { Id = 3, Name = "Finansman" }
            );

            // 2. Test Kullanıcıları
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser { Id = 1, RoleId = 1, FirstName = "Ahmet", LastName = "Memur", Email = "ahmet@sirket.com", IdentityNumber = "11111111111", RegistrationNumber = "S-001", PasswordHash = "123456" },
                new AppUser { Id = 2, RoleId = 2, FirstName = "Ayşe", LastName = "Yönetici", Email = "ayse@sirket.com", IdentityNumber = "22222222222", RegistrationNumber = "S-002", PasswordHash = "123456" },
                new AppUser { Id = 3, RoleId = 3, FirstName = "Mehmet", LastName = "Finans", Email = "mehmet@sirket.com", IdentityNumber = "33333333333", RegistrationNumber = "S-003", PasswordHash = "123456" }
            );

            // 3. Ceza Maddeleri (FineTypes)
            modelBuilder.Entity<FineType>().HasData(
                new FineType { Id = 1, ArticleNumber = "47/1-b", Description = "Kırmızı ışık kuralına uymamak", Amount = 1506 },
                new FineType { Id = 2, ArticleNumber = "51/2-a", Description = "Hız sınırını %10 - %30 aşmak", Amount = 1506 },
                new FineType { Id = 3, ArticleNumber = "51/2-b", Description = "Hız sınırını %30 - %50 aşmak", Amount = 3135 },
                new FineType { Id = 4, ArticleNumber = "51/2-c", Description = "Hız sınırını %50'den fazla aşmak", Amount = 6439 },
                new FineType { Id = 5, ArticleNumber = "78/1-a", Description = "Emniyet kemeri takmamak", Amount = 690 },
                new FineType { Id = 6, ArticleNumber = "73/c", Description = "Seyir halinde cep telefonu kullanmak", Amount = 1506 }
            );

            // 4. Marka ve Modeller (TSB Simülasyonu)
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
                new Model { Id = 3, BrandId = 2, Name = "Transit" },
                new Model { Id = 4, BrandId = 2, Name = "Focus" },
                new Model { Id = 5, BrandId = 3, Name = "Egea" },
                new Model { Id = 6, BrandId = 4, Name = "Corolla" },
                new Model { Id = 7, BrandId = 5, Name = "Passat" }
            );

            modelBuilder.Entity<Vehicle>().HasData(
                new Vehicle { Id = 1, BrandId = 1, ModelId = 1, PlateNumber = "34ABC123", VehicleType = trafficFineManager.Entities.Enums.VehicleType.Binek, OwnerName = "Ahmet Yılmaz", OwnerTC = "11111111110" },
                new Vehicle { Id = 2, BrandId = 2, ModelId = 3, PlateNumber = "06XYZ987", VehicleType = trafficFineManager.Entities.Enums.VehicleType.Kiralik, OwnerName = "ABC Rent A Car", OwnerTC = "22222222220" },
                new Vehicle { Id = 3, BrandId = 3, ModelId = 5, PlateNumber = "35DEF456", VehicleType = trafficFineManager.Entities.Enums.VehicleType.Binek, OwnerName = "Mehmet Demir", OwnerTC = "33333333330" }
            );
        }
    }
}