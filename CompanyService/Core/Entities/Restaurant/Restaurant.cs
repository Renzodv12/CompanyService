using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Entities.Restaurant
{
    public class Restaurant
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? City { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(100)]
        public string? Email { get; set; }
        
        [StringLength(20)]
        public string? RUC { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign Keys
        public Guid CompanyId { get; set; }
        public Guid CreatedBy { get; set; }
        
        // Navigation Properties
        public virtual Company Company { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<RestaurantTable> Tables { get; set; } = new List<RestaurantTable>();
        public virtual ICollection<RestaurantMenu> Menus { get; set; } = new List<RestaurantMenu>();
        public virtual ICollection<RestaurantOrder> Orders { get; set; } = new List<RestaurantOrder>();
    }
}

