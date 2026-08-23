using System.ComponentModel.DataAnnotations;

namespace trafficFineManager.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Lütfen Sicil No veya Kimlik No giriniz.")]
        [Display(Name = "Sicil No veya TC Kimlik No")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Lütfen şifrenizi giriniz.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = null!;
    }
}
