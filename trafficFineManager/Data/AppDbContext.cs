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

        // 2. Tablo İlişkileri ve Başlangıç Verileri (Seed Data)

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

            // --- YENİ EKLENEN KISIM: SQL Server Çoklu Silme Döngüsü Çözümü ---
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
            // ----------------------------------------------------------------

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

            // 3. Marka ve Modeller
            modelBuilder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "Renault" },
                new Brand { Id = 2, Name = "Ford" }
            );

            modelBuilder.Entity<Model>().HasData(
                new Model { Id = 1, BrandId = 1, Name = "Clio" },
                new Model { Id = 2, BrandId = 1, Name = "Megane" },
                new Model { Id = 3, BrandId = 2, Name = "Transit" }
            );
        }
    }
}