using System.ComponentModel.DataAnnotations;
using trafficFineManager.Entities.Enums;

namespace trafficFineManager.Entities
{
    public class TrafficFine
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
        public int CreatorUserId { get; set; }
        public AppUser CreatorUser { get; set; } = null!;
        [Required, MaxLength(200)]
        public string ViolationReason { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime NotificationDate { get; set; }
        public FineStatus Status { get; set; } = FineStatus.Yeni;
        [MaxLength(50)]
        public string? ReceiptNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<TrafficFineHistory> Histories { get; set; } = new List<TrafficFineHistory>();
    }
}
