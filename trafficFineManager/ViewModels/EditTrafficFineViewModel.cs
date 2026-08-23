using System.ComponentModel.DataAnnotations;
using trafficFineManager.Entities.Enums;

namespace trafficFineManager.ViewModels
{
    public class EditTrafficFineViewModel
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }

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

        [Required(ErrorMessage = "Lütfen ceza maddesini seçiniz.")]
        [Display(Name = "Ceza Maddesi")]
        public int FineTypeId { get; set; }

        [Required(ErrorMessage = "Sürücü Adı Soyadı zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Ceza Yiyen Sürücü Ad/Soyad")]
        public string ViolatorName { get; set; } = null!;

        [Required(ErrorMessage = "Lütfen sürücü TC numarasını giriniz.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır.")]
        [Display(Name = "Sürücü TC Kimlik No")]
        public string ViolatorTC { get; set; } = null!;

        [Required(ErrorMessage = "Lütfen ceza tarihini giriniz.")]
        [Display(Name = "Ceza Tarihi (Olay Zamanı)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ViolationDate { get; set; }

        [Required(ErrorMessage = "Lütfen il seçiniz.")]
        [Display(Name = "Ceza İli")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Lütfen ilçe seçiniz.")]
        [Display(Name = "Ceza İlçesi")]
        public int DistrictId { get; set; }

        [Required(ErrorMessage = "Makbuz numarası zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Makbuz Numarası")]
        public string ReceiptNumber { get; set; } = null!;
    }
}