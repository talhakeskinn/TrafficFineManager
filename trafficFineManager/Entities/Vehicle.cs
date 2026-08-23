using System.ComponentModel.DataAnnotations;
using trafficFineManager.Entities.Enums;

namespace trafficFineManager.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        [Required, MaxLength(15)]
        public string PlateNumber { get; set; } = null!;
        public VehicleType VehicleType { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;
        public int ModelId { get; set; }
        public Model Model { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}
