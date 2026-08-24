using FluentValidation;
using trafficFineManager.ViewModels;
using TrafficFineApp.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace trafficFineManager.Validators
{
    public class CreateTrafficFineValidator : AbstractValidator<CreateTrafficFineViewModel>
    {
        private readonly AppDbContext _context;

        public CreateTrafficFineValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(x => x.PlateNumber)
                .NotEmpty().WithMessage("Araç plakası zorunludur !")
                .Matches(@"^(0[1-9]|[1-7][0-9]|8[01])[a-zA-Z]{1,3}\d{2,4}$").WithMessage("Lütfen geçerli bir Türkiye plakası giriniz (Örn: 34ABC123).")
                .Must((model, plate) =>
                {
                    // 1. KURAL: Mükerrer Kayıt (Aynı plaka, aynı tarih, aynı ceza maddesi)
                    bool exists = _context.TrafficFines
                        .Include(t => t.Vehicle)
                        .Any(t => 
                            t.Vehicle.PlateNumber == plate &&
                            t.ViolationDate == model.ViolationDate &&
                            t.FineTypeId == model.FineTypeId);
                    
                    return !exists;
                }).WithMessage("MÜKERRER KAYIT: Bu araca aynı tarih ve saatte bu ceza maddesinden zaten bir kayıt açılmış!");

            RuleFor(x => x.ViolatorTC)
                .NotEmpty().WithMessage("Sürücü TC Kimlik Numarası zorunludur.")
                .Length(11).WithMessage("Sürücü TC Kimlik Numarası tam 11 haneli olmalıdır.")
                .Matches("^[0-9]*$").WithMessage("TC Kimlik Numarası sadece rakamlardan oluşabilir.");

            RuleFor(x => x.ViolatorName)
                .NotEmpty().WithMessage("Sürücü Ad/Soyad alanı zorunludur.")
                .MinimumLength(3).WithMessage("Sürücü adı çok kısa.")
                .Matches(@"^[a-zA-ZçÇğĞıİöÖşŞüÜ\s]*$").WithMessage("Sürücü adı sadece harflerden oluşmalıdır.");

            RuleFor(x => x.OwnerTC)
                .NotEmpty().WithMessage("Araç Sahibi TC Kimlik Numarası zorunludur.")
                .Length(11).WithMessage("Araç Sahibi TC Kimlik Numarası tam 11 haneli olmalıdır.")
                .Matches("^[0-9]*$").WithMessage("TC Kimlik Numarası sadece rakamlardan oluşabilir.");

            RuleFor(x => x.OwnerName)
                .NotEmpty().WithMessage("Araç Sahibi Ad/Soyad alanı zorunludur.")
                .MinimumLength(3).WithMessage("Araç Sahibi adı çok kısa.")
                .Matches(@"^[a-zA-ZçÇğĞıİöÖşŞüÜ\s]*$").WithMessage("Araç Sahibi adı sadece harflerden oluşmalıdır.");

            RuleFor(x => x.BrandId).GreaterThan(0).WithMessage("Lütfen bir araç markası seçiniz.");
            RuleFor(x => x.ModelId).GreaterThan(0).WithMessage("Lütfen bir araç modeli seçiniz.");
            RuleFor(x => x.CityId).GreaterThan(0).WithMessage("Lütfen cezanın kesildiği ili seçiniz.");
            RuleFor(x => x.DistrictId).GreaterThan(0).WithMessage("Lütfen cezanın kesildiği ilçeyi seçiniz.");
            
            RuleFor(x => x.FineTypeId)
                .GreaterThan(0).WithMessage("Lütfen bir ceza maddesi seçiniz.")
                .Must((model, fineTypeId) =>
                {
                    // 2. KURAL: Araç Tipi ve Ceza Maddesi Uyumu
                    var fineType = _context.FineTypes.Find(fineTypeId);
                    if (fineType != null)
                    {
                        var desc = fineType.Description.ToLower();
                        if (desc.Contains("tonaj") && model.VehicleType != trafficFineManager.Entities.Enums.VehicleType.Cekici && model.VehicleType != trafficFineManager.Entities.Enums.VehicleType.Dorse)
                            return false;
                    }
                    return true;
                }).WithMessage("MANTIK HATASI: Seçtiğiniz ceza maddesi, bu araç tipine uygulanamaz.");

            RuleFor(x => x.ReceiptNumber)
                .NotEmpty().WithMessage("Makbuz numarası boş olamaz.");

            RuleFor(x => x.ViolationDate)
                .NotEmpty().WithMessage("Ceza tarihi zorunludur.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Gelecek bir tarihe ceza makbuzu kesilemez.");
        }
    }
}


