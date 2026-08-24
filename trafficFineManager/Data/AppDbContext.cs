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
                new AppRole { Id = 1, Name = "StandartKullanici" },
                new AppRole { Id = 2, Name = "Yonetici" },
                new AppRole { Id = 3, Name = "Finansman" }
            );

            modelBuilder.Entity<AppUser>().HasData(
                new AppUser { Id = 1, RoleId = 1, RegistrationNumber = "S-001", IdentityNumber = "11111111111", FirstName = "Ahmet", LastName = "StandartKullanici", Email = "ahmet@test.com", PasswordHash = "123456" },
                new AppUser { Id = 2, RoleId = 2, RegistrationNumber = "S-002", IdentityNumber = "22222222222", FirstName = "Ayşe", LastName = "Yönetici", Email = "ayse@test.com", PasswordHash = "123456" },
                new AppUser { Id = 3, RoleId = 3, RegistrationNumber = "S-003", IdentityNumber = "33333333333", FirstName = "Fatma", LastName = "Finans", Email = "fatma@test.com", PasswordHash = "123456" }
            );

            modelBuilder.Entity<FineType>().HasData(
                new FineType { Id = 1, ArticleNumber = "47/1-b", Description = "Kırmızı ışık kuralına uymamak", Amount = 1506.00M, IsActive = true },
                new FineType { Id = 2, ArticleNumber = "51/2-a", Description = "Hız sınırını %10'dan %30'a kadar aşmak", Amount = 1506.00M, IsActive = true },
                new FineType { Id = 3, ArticleNumber = "51/2-b", Description = "Hız sınırını %30'dan %50'ye kadar aşmak", Amount = 3135.00M, IsActive = true },
                new FineType { Id = 4, ArticleNumber = "73/c", Description = "Seyir halinde cep telefonu kullanmak", Amount = 1506.00M, IsActive = true },
                new FineType { Id = 5, ArticleNumber = "48/5", Description = "Alkollü araç kullanmak (1. Defa)", Amount = 6439.00M, IsActive = true }                 ,
                new FineType { Id = 6, ArticleNumber = "78/1-a", Description = "Emniyet kemeri bulundurmamak ve kullanmamak", Amount = 690.00M, IsActive = true },
                new FineType { Id = 7, ArticleNumber = "34/a", Description = "Muayenesi yapılmamış bir aracın trafiğe çıkarılması", Amount = 1506.00M, IsActive = true },
                new FineType { Id = 8, ArticleNumber = "61/1-a", Description = "Taşıt yolu üzerinde hatalı park etmek", Amount = 690.00M, IsActive = true },
                new FineType { Id = 9, ArticleNumber = "67/1-d", Description = "Drift atmak (Araçla tehlikeli hareketler yapmak)", Amount = 32233.00M, IsActive = true },
                new FineType { Id = 10, ArticleNumber = "26/2", Description = "Araçlarda yetkisiz çakar veya siren kullanmak", Amount = 6439.00M, IsActive = true }
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

            
        }
    }
}






