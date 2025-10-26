using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Entities.Restaurant
{
    public class RestaurantMenu
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign Keys
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid CreatedBy { get; set; }
        
        // Navigation Properties
        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<RestaurantMenuItem> MenuItems { get; set; } = new List<RestaurantMenuItem>();
    }
}

