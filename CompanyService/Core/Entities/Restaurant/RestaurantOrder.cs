using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Entities.Restaurant
{
    public class RestaurantOrder
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;
        
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public OrderType Type { get; set; } = OrderType.DineIn;
        
        [StringLength(200)]
        public string? CustomerName { get; set; }
        
        [StringLength(20)]
        public string? CustomerPhone { get; set; }
        
        public int NumberOfGuests { get; set; } = 1;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [StringLength(500)]
        public string? SpecialInstructions { get; set; }
        
        public DateTime OrderTime { get; set; } = DateTime.UtcNow;
        public DateTime? EstimatedReadyTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign Keys
        public Guid RestaurantId { get; set; }
        public Guid TableId { get; set; }
        public Guid SaleId { get; set; } // Reference to the actual Sale
        public Guid CreatedBy { get; set; }
        public Guid? AssignedWaiterId { get; set; }
        
        // Navigation Properties
        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual RestaurantTable Table { get; set; } = null!;
        public virtual Sale Sale { get; set; } = null!; // Link to existing Sale
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual User? AssignedWaiter { get; set; }
    }
    
    public enum OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        Preparing = 3,
        Ready = 4,
        Served = 5,
        Completed = 6,
        Cancelled = 7
    }
    
    public enum OrderType
    {
        DineIn = 1,
        TakeOut = 2,
        Delivery = 3
    }
}

