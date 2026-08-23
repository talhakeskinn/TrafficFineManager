using System.ComponentModel.DataAnnotations;

namespace trafficFineManager.ViewModels
{
    public class RejectFineViewModel
    {
        public int TrafficFineId { get; set; }
        [Required(ErrorMessage = "Lütfen ret nedenini belirtiniz.")]
        [StringLength(500, ErrorMessage = "Ret nedeni en fazla 500 karakter olabilir.")]
        public string RejectReason { get; set; } = null!;
    }
}
