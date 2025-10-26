using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.Restaurant
{
    public class RestaurantMenuItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public int? PreparationTime { get; set; }
        public string? Allergens { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }
        public bool IsGlutenFree { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
    }

    public class CreateRestaurantMenuItemRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [StringLength(100)]
        public string? Category { get; set; }
        
        [StringLength(200)]
        public string? ImageUrl { get; set; }
        
        [Range(1, 300)]
        public int? PreparationTime { get; set; } = 15;
        
        [StringLength(100)]
        public string? Allergens { get; set; }
        
        public bool IsVegetarian { get; set; } = false;
        public bool IsVegan { get; set; } = false;
        public bool IsGlutenFree { get; set; } = false;
    }

    public class UpdateRestaurantMenuItemRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [StringLength(100)]
        public string? Category { get; set; }
        
        [StringLength(200)]
        public string? ImageUrl { get; set; }
        
        [Range(1, 300)]
        public int? PreparationTime { get; set; }
        
        [StringLength(100)]
        public string? Allergens { get; set; }
        
        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsAvailable { get; set; }
    }
}

