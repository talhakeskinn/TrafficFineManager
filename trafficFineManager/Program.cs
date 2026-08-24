using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TrafficFineApp.Data;
using trafficFineManager.Services;
using trafficFineManager.Services.Abstraction;
using trafficFineManager.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITrafficFineService, TrafficFineService>();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddFluentValidationAutoValidation(); 
builder.Services.AddValidatorsFromAssemblyContaining<CreateTrafficFineValidator>(); 

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.Migrate();

    if (context.Cities.Count() < 81)
    {
        var citiesList = new string[] {
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
                context.Cities.Add(new trafficFineManager.Entities.City { Id = cityId, Name = citiesList[i] });
                needsInsert = true;
            }
        }
        
        if (needsInsert)
        {
            context.Database.OpenConnection();
            try {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Cities ON");
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Cities OFF");
            } catch {

                context.SaveChanges();
            } finally {
                context.Database.CloseConnection();
            }
        }
    }

    if (context.Districts.Count() < 100)
    {
        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData", "ilce.json");
        if (System.IO.File.Exists(jsonPath))
        {
            var jsonString = System.IO.File.ReadAllText(jsonPath);
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
                                    context.Districts.Add(new trafficFineManager.Entities.District 
                                    { 
                                        Name = ilceAdi, 
                                        CityId = ilId 
                                    });
                                }
                            }
                        }
                    }
                }
            }
            context.SaveChanges();
        }
    }

    if (context.Brands.Count() < 150)
    {
        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData", "arac_listesi.json");
        if (System.IO.File.Exists(jsonPath))
        {
            var jsonString = System.IO.File.ReadAllText(jsonPath);
            using var doc = System.Text.Json.JsonDocument.Parse(jsonString);
            
            foreach (var brandProperty in doc.RootElement.EnumerateObject())
            {
                var bName = brandProperty.Name.Length > 50 ? brandProperty.Name.Substring(0, 50) : brandProperty.Name;
                
                var brand = context.Brands.FirstOrDefault(b => b.Name == bName);
                if (brand == null)
                {
                    brand = new trafficFineManager.Entities.Brand { Name = bName };
                    context.Brands.Add(brand);
                    context.SaveChanges();
                }
                
                var existingModelNames = context.Models.Where(m => m.BrandId == brand.Id).Select(m => m.Name).ToList();
                var modelsToAdd = new System.Collections.Generic.List<trafficFineManager.Entities.Model>();
                
                foreach (var modelProperty in brandProperty.Value.EnumerateObject())
                {
                    var mName = modelProperty.Name.Length > 50 ? modelProperty.Name.Substring(0, 50) : modelProperty.Name;
                    if (!existingModelNames.Contains(mName))
                    {
                        modelsToAdd.Add(new trafficFineManager.Entities.Model { Name = mName, BrandId = brand.Id });
                        existingModelNames.Add(mName);
                    }
                }
                
                if (modelsToAdd.Any())
                {
                    context.Models.AddRange(modelsToAdd);
                    context.SaveChanges();
                }
            }
        }
    }

    if (context.Vehicles.Count() < 5)
    {
        var random = new Random();
        var brands = context.Brands.ToList();
        var models = context.Models.ToList();
        var cities = context.Cities.ToList();
        var districts = context.Districts.ToList();
        var fineTypes = context.FineTypes.ToList();

        if (brands.Any() && models.Any() && cities.Any() && districts.Any() && fineTypes.Any())
        {
            var vehicles = new List<trafficFineManager.Entities.Vehicle>();
            for (int i = 1; i <= 10; i++)
            {
                var rBrand = brands[random.Next(brands.Count)];
                var bModels = models.Where(m => m.BrandId == rBrand.Id).ToList();
                if(!bModels.Any()) continue;
                var rModel = bModels[random.Next(bModels.Count)];

                var v = new trafficFineManager.Entities.Vehicle
                {
                    BrandId = rBrand.Id,
                    ModelId = rModel.Id,
                    PlateNumber = $"34ABC{100 + i}",
                    VehicleType = trafficFineManager.Entities.Enums.VehicleType.Binek,
                    OwnerName = $"Test Sahibi {i}",
                    OwnerTC = $"111111111{i:00}",
                    IsActive = true
                };
                context.Vehicles.Add(v);
                vehicles.Add(v);
            }
            context.SaveChanges();

            var pastDate = DateTime.Now.AddDays(-30);
            var fines = new List<trafficFineManager.Entities.TrafficFine>();
            var fineStatuses = new[] 
            {
                trafficFineManager.Entities.Enums.FineStatus.Yeni,
                trafficFineManager.Entities.Enums.FineStatus.Yeni,
                trafficFineManager.Entities.Enums.FineStatus.YoneticiOnayinda,
                trafficFineManager.Entities.Enums.FineStatus.YoneticiOnayinda,
                trafficFineManager.Entities.Enums.FineStatus.FinansOnayinda,
                trafficFineManager.Entities.Enums.FineStatus.FinansOnayinda,
                trafficFineManager.Entities.Enums.FineStatus.Reddedildi,
                trafficFineManager.Entities.Enums.FineStatus.Tamamlandi,
                trafficFineManager.Entities.Enums.FineStatus.Tamamlandi,
                trafficFineManager.Entities.Enums.FineStatus.Tamamlandi
            };

            for (int i = 0; i < 20; i++)
            {
                var v = vehicles[random.Next(vehicles.Count)];
                var fType = fineTypes[random.Next(fineTypes.Count)];
                var c = cities[random.Next(cities.Count)];
                var d = districts.Where(x => x.CityId == c.Id).FirstOrDefault() ?? districts.First();
                var status = fineStatuses[i % fineStatuses.Length];

                var fine = new trafficFineManager.Entities.TrafficFine
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
                    ReceiptNumber = $"TR-2026-1{i:00}",
                    CreatorUserId = 1
                };
                context.TrafficFines.Add(fine);
                fines.Add(fine);
            }
            context.SaveChanges();

            foreach(var f in fines)
            {
                context.TrafficFineHistories.Add(new trafficFineManager.Entities.TrafficFineHistory
                {
                    TrafficFineId = f.Id,
                    UserId = 1,
                    ActionType = trafficFineManager.Entities.Enums.ActionType.Olusturuldu,
                    OldStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni,
                    NewStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni,
                    Description = "Sisteme eklendi.",
                    ActionDate = f.CreatedAt
                });

                if (f.Status >= trafficFineManager.Entities.Enums.FineStatus.YoneticiOnayinda)
                {
                    context.TrafficFineHistories.Add(new trafficFineManager.Entities.TrafficFineHistory
                    {
                        TrafficFineId = f.Id,
                        UserId = 2,
                        ActionType = trafficFineManager.Entities.Enums.ActionType.Onaylandi,
                        OldStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni,
                        NewStatus = trafficFineManager.Entities.Enums.FineStatus.YoneticiOnayinda,
                        Description = "Yönetici onayı verildi.",
                        ActionDate = f.CreatedAt.AddHours(2)
                    });
                }
                if (f.Status >= trafficFineManager.Entities.Enums.FineStatus.FinansOnayinda && f.Status != trafficFineManager.Entities.Enums.FineStatus.Reddedildi)
                {
                    context.TrafficFineHistories.Add(new trafficFineManager.Entities.TrafficFineHistory
                    {
                        TrafficFineId = f.Id,
                        UserId = 3,
                        ActionType = trafficFineManager.Entities.Enums.ActionType.Onaylandi,
                        OldStatus = trafficFineManager.Entities.Enums.FineStatus.YoneticiOnayinda,
                        NewStatus = trafficFineManager.Entities.Enums.FineStatus.FinansOnayinda,
                        Description = "Finans onayı verildi.",
                        ActionDate = f.CreatedAt.AddHours(4)
                    });
                }
                if (f.Status == trafficFineManager.Entities.Enums.FineStatus.Tamamlandi)
                {
                    context.TrafficFineHistories.Add(new trafficFineManager.Entities.TrafficFineHistory
                    {
                        TrafficFineId = f.Id,
                        UserId = 3,
                        ActionType = trafficFineManager.Entities.Enums.ActionType.Onaylandi,
                        OldStatus = trafficFineManager.Entities.Enums.FineStatus.FinansOnayinda,
                        NewStatus = trafficFineManager.Entities.Enums.FineStatus.Tamamlandi,
                        Description = "Tahsilat gerçekleşti. Kapatıldı.",
                        ActionDate = f.CreatedAt.AddDays(1)
                    });
                }
                if (f.Status == trafficFineManager.Entities.Enums.FineStatus.Reddedildi)
                {
                    context.TrafficFineHistories.Add(new trafficFineManager.Entities.TrafficFineHistory
                    {
                        TrafficFineId = f.Id,
                        UserId = 2,
                        ActionType = trafficFineManager.Entities.Enums.ActionType.Reddedildi,
                        OldStatus = trafficFineManager.Entities.Enums.FineStatus.Yeni,
                        NewStatus = trafficFineManager.Entities.Enums.FineStatus.Reddedildi,
                        Description = "Plaka okunamıyor, reddedildi.",
                        ActionDate = f.CreatedAt.AddHours(2)
                    });
                }
            }
            context.SaveChanges();
        }
    }
}

app.Run();


