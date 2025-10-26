using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.Restaurant
{
    public class ChangeTableStatusRequest
    {
        [Required]
        public int Status { get; set; }
        
        [StringLength(200)]
        public string? Notes { get; set; }
    }
}
