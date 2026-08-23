using System.ComponentModel.DataAnnotations;

namespace trafficFineManager.Entities
{
    public class FineType
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string ArticleNumber { get; set; } = null!; // Madde No (Örn: 47/1-b)

        [Required, MaxLength(250)]
        public string Description { get; set; } = null!; // İhlal Nedeni

        public decimal Amount { get; set; } // Ceza Tutarı
        
        public bool IsActive { get; set; } = true;
    }
}
