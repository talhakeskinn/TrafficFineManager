using System.ComponentModel.DataAnnotations;
using trafficFineManager.Entities.Enums;

namespace trafficFineManager.Entities
{
    // Ensure Enums namespace is correct
}

namespace trafficFineManager.ViewModels
{
    public class CreateTrafficFineViewModel
    {
        // --- ARAÇ BİLGİLERİ ---
        [Required(ErrorMessage = "Plaka zorunludur.")]
        [StringLength(15)]
        [Display(Name = "Plaka")]
        public string PlateNumber { get; set; } = null!;

        [Required(ErrorMessage = "Lütfen marka seçiniz.")]
        [Display(Name = "Marka")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Lütfen model seçiniz.")]
        [Display(Name = "Model")]
        public int ModelId { get; set; }

        [Required(ErrorMessage = "Lütfen araç tipi seçiniz.")]
        [Display(Name = "Araç Tipi")]
        public VehicleType VehicleType { get; set; }

        [Required(ErrorMessage = "Araç Sahibi Adı/Soyadı zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Araç Sahibi (Ruhsat Sahibi)")]
        public string OwnerName { get; set; } = null!;

        [Required(ErrorMessage = "Araç Sahibi TC zorunludur.")]
        [StringLength(11, MinimumLength = 11)]
        [Display(Name = "Araç Sahibi TC Kimlik No")]
        public string OwnerTC { get; set; } = null!;

        // --- CEZA VE SÜRÜCÜ BİLGİLERİ ---
        [Required(ErrorMessage = "Lütfen ceza maddesini seçiniz.")]
        [Display(Name = "Ceza Maddesi")]
        public int FineTypeId { get; set; }

        [Required(ErrorMessage = "Sürücü Adı Soyadı zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Ceza Yiyen Sürücü Ad/Soyad")]
        public string ViolatorName { get; set; } = null!;

        [Required(ErrorMessage = "Sürücü TC Kimlik No zorunludur.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır.")]
        [Display(Name = "Sürücü TC Kimlik No")]
        public string ViolatorTC { get; set; } = null!;

        [Required(ErrorMessage = "Makbuz numarası zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Makbuz Numarası")]
        public string ReceiptNumber { get; set; } = null!;
    }
}