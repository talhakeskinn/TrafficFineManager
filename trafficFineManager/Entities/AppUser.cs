using System.ComponentModel.DataAnnotations;

namespace trafficFineManager.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public AppRole Role { get; set; } = null!;

        [Required, MaxLength(11)]
        public string IdentityNumber { get; set; } = null!;

        [Required, MaxLength(20)]
        public string RegistrationNumber { get; set; } = null!;

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}
