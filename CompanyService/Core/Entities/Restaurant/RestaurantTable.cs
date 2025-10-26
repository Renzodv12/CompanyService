using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Entities.Restaurant
{
    public class RestaurantTable
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string TableNumber { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Name { get; set; }
        
        public int Capacity { get; set; } = 4;
        
        public TableStatus Status { get; set; } = TableStatus.Available;
        
        [StringLength(200)]
        public string? Location { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign Keys
        public Guid RestaurantId { get; set; }
        public Guid? CurrentOrderId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid CreatedBy { get; set; }
        
        // Navigation Properties
        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual RestaurantOrder? CurrentOrder { get; set; }
        public virtual ICollection<RestaurantOrder> Orders { get; set; } = new List<RestaurantOrder>();
    }
    
    public enum TableStatus
    {
        Available = 1,
        Occupied = 2,
        Reserved = 3,
        Cleaning = 4,
        OutOfService = 5
    }
}

