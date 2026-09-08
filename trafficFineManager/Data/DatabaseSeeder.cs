using Microsoft.EntityFrameworkCore;
using TrafficFineApp.Data;
using trafficFineManager.Entities;
using trafficFineManager.Entities.Enums;

namespace trafficFineManager.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.Migrate();

            await SeedCitiesAsync(context);
            await SeedDistrictsAsync(context);
            await SeedBrandsAndModelsAsync(context);
            await SeedTestDataAsync(context);
        }

        private static async Task SeedCitiesAsync(AppDbContext context)
        {
            if (context.Cities.Count() >= 81) return;

            var citiesList = new string[]
            {
                "Adana", "Adıyaman", "Afyonkarahisar", "Ağrı", "Amasya", "Ankara", "Antalya", "Artvin", "Aydın", "Balıkesir",
                "Bilecik", "Bingöl", "Bitlis", "Bolu", "Burdur", "Bursa", "Çanakkale", "Çankırı", "Çorum", "Denizli",
                "Diyarbakır", "Edirne", "Elazığ", "Erzincan", "Erzurum", "Eskişehir", "Gaziantep", "Giresun", "Gümüşhane",
                "Hakkari", "Hatay", "Isparta", "Mersin", "İstanbul", "İzmir", "Kars", "Kastamonu", "Kayseri", "Kırklareli",
                "Kırşehir", "Kocaeli", "Konya", "Kütahya", "Malatya", "Manisa", "Kahramanmaraş", "Mardin", "Muğla", "Muş",
                "Nevşehir", "Niğde", "Ordu", "Rize", "Sakarya", "Samsun", "Siirt", "Sinop", "Sivas", "Tekirdağ", "Tokat",
                "Trabzon", "Tunceli", "Şanlıurfa", "Uşak", "Van", "Yozgat", "Zonguldak", "Aksaray", "Bayburt", "Karaman",
                "Kırıkkale", "Batman", "Şırnak", "Bartın", "Ardahan", "Iğdır", "Yalova", "Karabük", "Kilis", "Osmaniye", "Düzce"
            };

            bool needsInsert = false;
            for (int i = 0; i < citiesList.Length; i++)
            {
                int cityId = i + 1;
                if (!context.Cities.Any(c => c.Id == cityId))
                {
                    context.Cities.Add(new City { Id = cityId, Name = citiesList[i] });
                    needsInsert = true;
                }
            }

            if (needsInsert)
            {
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Cities ON");
                    await context.SaveChangesAsync();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Cities OFF");
                }
                catch
                {
                    await context.SaveChangesAsync();
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }
        }

        private static async Task SeedDistrictsAsync(AppDbContext context)
        {
            if (context.Districts.Count() >= 100) return;

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData", "ilce.json");
            if (!File.Exists(jsonPath)) return;

            var jsonString = File.ReadAllText(jsonPath);
            using var doc = System.Text.Json.JsonDocument.Parse(jsonString);

            foreach (var element in doc.RootElement.EnumerateArray())
            {
                if (element.TryGetProperty("type", out var type) && type.GetString() == "table" &&
                    element.TryGetProperty("name", out var tableName) && tableName.GetString() == "ilce" &&
                    element.TryGetProperty("data", out var dataArray))
                {
                    foreach (var row in dataArray.EnumerateArray())
                    {
                        var ilceAdi = row.GetProperty("name").GetString();
                        var ilIdString = row.GetProperty("il_id").GetString();

                        if (ilceAdi != null && int.TryParse(ilIdString, out int ilId))
                        {
                            if (!context.Districts.Any(d => d.Name == ilceAdi && d.CityId == ilId))
                            {
                                if (context.Cities.Any(c => c.Id == ilId))
                                {
                                    context.Districts.Add(new District { Name = ilceAdi, CityId = ilId });
                                }
                            }
                        }
                    }
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedBrandsAndModelsAsync(AppDbContext context)
        {
            if (context.Brands.Count() >= 150) return;

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData", "arac_listesi.json");
            if (!File.Exists(jsonPath)) return;

            var jsonString = File.ReadAllText(jsonPath);
            using var doc = System.Text.Json.JsonDocument.Parse(jsonString);

            foreach (var brandProperty in doc.RootElement.EnumerateObject())
            {
                var bName = brandProperty.Name.Length > 50 ? brandProperty.Name.Substring(0, 50) : brandProperty.Name;

                var brand = context.Brands.FirstOrDefault(b => b.Name == bName);
                if (brand == null)
                {
                    brand = new Brand { Name = bName };
                    context.Brands.Add(brand);
                    await context.SaveChangesAsync();
                }

                var existingModelNames = context.Models.Where(m => m.BrandId == brand.Id).Select(m => m.Name).ToList();
                var modelsToAdd = new List<Model>();

                foreach (var modelProperty in brandProperty.Value.EnumerateObject())
                {
                    var mName = modelProperty.Name.Length > 50 ? modelProperty.Name.Substring(0, 50) : modelProperty.Name;
                    if (!existingModelNames.Contains(mName))
                    {
                        modelsToAdd.Add(new Model { Name = mName, BrandId = brand.Id });
                        existingModelNames.Add(mName);
                    }
                }

                if (modelsToAdd.Any())
                {
                    context.Models.AddRange(modelsToAdd);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedTestDataAsync(AppDbContext context)
        {
            if (context.Vehicles.Count() >= 5) return;

            var random = new Random();
            var brands = context.Brands.ToList();
            var models = context.Models.ToList();
            var cities = context.Cities.ToList();
            var districts = context.Districts.ToList();
            var fineTypes = context.FineTypes.ToList();

            if (!brands.Any() || !models.Any() || !cities.Any() || !districts.Any() || !fineTypes.Any()) return;

            var vehicles = new List<Vehicle>();
            for (int i = 1; i <= 10; i++)
            {
                var rBrand = brands[random.Next(brands.Count)];
                var bModels = models.Where(m => m.BrandId == rBrand.Id).ToList();
                if (!bModels.Any()) continue;
                var rModel = bModels[random.Next(bModels.Count)];

                var v = new Vehicle
                {
                    BrandId = rBrand.Id,
                    ModelId = rModel.Id,
                    PlateNumber = $"34ABC{100 + i}",
                    VehicleType = VehicleType.Binek,
                    OwnerName = $"Test Sahibi {i}",
                    OwnerTC = $"111111111{i:00}",
                    IsActive = true
                };
                context.Vehicles.Add(v);
                vehicles.Add(v);
            }
            await context.SaveChangesAsync();

            var pastDate = DateTime.Now.AddDays(-30);
            var fineStatuses = new[]
            {
                FineStatus.Yeni,
                FineStatus.Yeni,
                FineStatus.YoneticiOnayinda,
                FineStatus.YoneticiOnayinda,
                FineStatus.FinansOnayinda,
                FineStatus.FinansOnayinda,
                FineStatus.Reddedildi,
                FineStatus.Tamamlandi,
                FineStatus.Tamamlandi,
                FineStatus.Tamamlandi
            };

            var fines = new List<TrafficFine>();
            for (int i = 0; i < 20; i++)
            {
                var v = vehicles[random.Next(vehicles.Count)];
                var fType = fineTypes[random.Next(fineTypes.Count)];
                var c = cities[random.Next(cities.Count)];
                var d = districts.Where(x => x.CityId == c.Id).FirstOrDefault() ?? districts.First();
                var status = fineStatuses[i % fineStatuses.Length];

                var fine = new TrafficFine
                {
                    VehicleId = v.Id,
                    FineTypeId = fType.Id,
                    ViolatorName = $"Sürücü {i}",
                    ViolatorTC = $"999999999{i:00}",
                    CityId = c.Id,
                    DistrictId = d.Id,
                    ViolationReason = fType.Description,
                    Amount = fType.Amount,
                    ViolationDate = pastDate.AddDays(i),
                    NotificationDate = pastDate.AddDays(i).AddHours(1),
                    CreatedAt = pastDate.AddDays(i).AddHours(1),
                    Status = status,
                    ReceiptNumber = $"MKZ-{DateTime.Now.Year}-{(i + 1):D6}",
                    CreatorUserId = 1
                };
                context.TrafficFines.Add(fine);
                fines.Add(fine);
            }
            await context.SaveChangesAsync();

            foreach (var f in fines)
            {
                CreateHistoryForFine(context, f);
            }
            await context.SaveChangesAsync();
        }

        private static void CreateHistoryForFine(AppDbContext context, TrafficFine fine)
        {
            context.TrafficFineHistories.Add(new TrafficFineHistory
            {
                TrafficFineId = fine.Id,
                UserId = 1,
                ActionType = ActionType.Olusturuldu,
                OldStatus = FineStatus.Yeni,
                NewStatus = FineStatus.Yeni,
                Description = "Ceza sisteme eklendi.",
                ActionDate = fine.CreatedAt
            });

            if (fine.Status >= FineStatus.YoneticiOnayinda && fine.Status != FineStatus.Reddedildi)
            {
                context.TrafficFineHistories.Add(new TrafficFineHistory
                {
                    TrafficFineId = fine.Id,
                    UserId = 2,
                    ActionType = ActionType.Onaylandi,
                    OldStatus = FineStatus.Yeni,
                    NewStatus = FineStatus.YoneticiOnayinda,
                    Description = "Standart kullanıcı tarafından iletildi. Yönetici incelemesinde.",
                    ActionDate = fine.CreatedAt.AddHours(2)
                });
            }

            if (fine.Status >= FineStatus.FinansOnayinda && fine.Status != FineStatus.Reddedildi)
            {
                context.TrafficFineHistories.Add(new TrafficFineHistory
                {
                    TrafficFineId = fine.Id,
                    UserId = 2,
                    ActionType = ActionType.Onaylandi,
                    OldStatus = FineStatus.YoneticiOnayinda,
                    NewStatus = FineStatus.FinansOnayinda,
                    Description = "Yönetici onayı verildi. Finans onayı bekleniyor.",
                    ActionDate = fine.CreatedAt.AddHours(4)
                });
            }

            if (fine.Status == FineStatus.Tamamlandi)
            {
                context.TrafficFineHistories.Add(new TrafficFineHistory
                {
                    TrafficFineId = fine.Id,
                    UserId = 3,
                    ActionType = ActionType.Onaylandi,
                    OldStatus = FineStatus.FinansOnayinda,
                    NewStatus = FineStatus.Tamamlandi,
                    Description = "Finans onayı verildi. Tahsilat gerçekleşti, işlem kapatıldı.",
                    ActionDate = fine.CreatedAt.AddDays(1)
                });
            }

            if (fine.Status == FineStatus.Reddedildi)
            {
                context.TrafficFineHistories.Add(new TrafficFineHistory
                {
                    TrafficFineId = fine.Id,
                    UserId = 2,
                    ActionType = ActionType.Reddedildi,
                    OldStatus = FineStatus.Yeni,
                    NewStatus = FineStatus.Reddedildi,
                    Description = "Plaka okunamıyor, reddedildi.",
                    ActionDate = fine.CreatedAt.AddHours(2)
                });
            }
        }
    }
}
