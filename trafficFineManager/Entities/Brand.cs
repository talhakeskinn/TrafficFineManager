using System.ComponentModel.DataAnnotations;

namespace trafficFineManager.Entities
{
    public class Brand
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;
    }
    public class Model
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;
        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;
    }
}
