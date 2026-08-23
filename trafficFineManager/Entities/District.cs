using System.ComponentModel.DataAnnotations;

namespace trafficFineManager.Entities
{
    public class District
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public City City { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;
    }
}
