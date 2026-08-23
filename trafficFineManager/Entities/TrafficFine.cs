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
        
        public int FineTypeId { get; set; }
        public FineType FineType { get; set; } = null!;

        // Yeni Eklenenler: Cezayı Yiyen Kişi Bilgileri
        [Required, MaxLength(100)]
        public string ViolatorName { get; set; } = null!;
        
        [Required, MaxLength(11)]
        public string ViolatorTC { get; set; } = null!;

        // Geçmişe dönük kayıtların bozulmaması için (Snapshot)
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
