using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace trafficFineManager.Entities
{
    public class FineType
    {
        public int Id { get; set; }
        
        [Required, MaxLength(20)]
        public string ArticleNumber { get; set; } = null!;

        [Required, MaxLength(500)]
        public string Description { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
