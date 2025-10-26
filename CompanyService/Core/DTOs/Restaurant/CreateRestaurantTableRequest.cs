using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.Restaurant
{
    public class CreateRestaurantTableRequest
    {
        [Required]
        [StringLength(50)]
        public string TableNumber { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Name { get; set; }
        
        [Range(1, 20)]
        public int Capacity { get; set; } = 4;
        
        [StringLength(200)]
        public string? Location { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
    }
}
