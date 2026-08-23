using System.ComponentModel.DataAnnotations;

namespace trafficFineManager.Entities
{
    public class AppRole
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;
        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    }
}
