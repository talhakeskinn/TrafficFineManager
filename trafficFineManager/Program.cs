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
    
    // Seed Cities if not completely seeded
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
                // Fallback
                context.SaveChanges();
            } finally {
                context.Database.CloseConnection();
            }
        }
    }

    // Seed Districts if mostly empty (we have a few test ones from HasData)
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
                            // Avoid exact duplicates
                            if (!context.Districts.Any(d => d.Name == ilceAdi && d.CityId == ilId))
                            {
                                // Only add if the referenced City exists!
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
}

app.Run();
