using System.ComponentModel.DataAnnotations;
using trafficFineManager.Entities.Enums;

namespace trafficFineManager.Entities
{
    public class TrafficFineHistory
    {
        public int Id { get; set; }
        public int TrafficFineId { get; set; }
        public TrafficFine TrafficFine { get; set; } = null!;
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public DateTime ActionDate { get; set; } = DateTime.Now;
        public ActionType ActionType { get; set; }
        public FineStatus? OldStatus { get; set; }
        public FineStatus NewStatus { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
