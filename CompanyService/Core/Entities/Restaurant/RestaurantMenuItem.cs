using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Entities.Restaurant
{
    public class RestaurantMenuItem
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(100)]
        public string? Category { get; set; }
        
        public decimal Price { get; set; }
        
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        public int? PreparationTime { get; set; } // in minutes
        
        [StringLength(200)]
        public string? Allergens { get; set; }
        
        public bool IsVegetarian { get; set; } = false;
        
        public bool IsVegan { get; set; } = false;
        
        public bool IsGlutenFree { get; set; } = false;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign Keys
        public Guid MenuId { get; set; }
        public Guid ProductId { get; set; } // Reference to the actual Product
        public Guid RestaurantId { get; set; } // Reference to Restaurant for easier querying
        public Guid CompanyId { get; set; }
        public Guid CreatedBy { get; set; }
        
        // Navigation Properties
        public virtual RestaurantMenu Menu { get; set; } = null!;
        public virtual Product Product { get; set; } = null!; // Link to existing Product
        public virtual Company Company { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
    }
}

