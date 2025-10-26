using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.Restaurant
{
    public class RestaurantOrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public string? SpecialInstructions { get; set; }
        public DateTime OrderTime { get; set; }
        public DateTime? EstimatedReadyTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public Guid TableId { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public Guid? AssignedWaiterId { get; set; }
        public string? AssignedWaiterName { get; set; }
        public List<RestaurantOrderItemDto> OrderItems { get; set; } = new();
        public List<RestaurantPaymentDto> Payments { get; set; } = new();
    }

    public class RestaurantOrderItemDto
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? SpecialInstructions { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid OrderId { get; set; }
        public Guid MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public string? MenuItemCategory { get; set; }
    }

    public class CreateRestaurantOrderRequest
    {
        [Required]
        public Guid TableId { get; set; }
        
        [Required]
        public int Type { get; set; }
        
        [StringLength(200)]
        public string? CustomerName { get; set; }
        
        [StringLength(20)]
        public string? CustomerPhone { get; set; }
        
        [Range(1, 20)]
        public int NumberOfGuests { get; set; } = 1;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [StringLength(500)]
        public string? SpecialInstructions { get; set; }
        
        [Required]
        public List<CreateOrderItemRequest> OrderItems { get; set; } = new();
    }

    public class CreateOrderItemRequest
    {
        [Required]
        public Guid MenuItemId { get; set; }
        
        [Required]
        [Range(1, 50)]
        public int Quantity { get; set; }
        
        [StringLength(200)]
        public string? SpecialInstructions { get; set; }
    }

    public class UpdateRestaurantOrderRequest
    {
        [Required]
        public int Status { get; set; }
        
        [StringLength(200)]
        public string? CustomerName { get; set; }
        
        [StringLength(20)]
        public string? CustomerPhone { get; set; }
        
        [Range(1, 20)]
        public int NumberOfGuests { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [StringLength(500)]
        public string? SpecialInstructions { get; set; }
        
        public Guid? AssignedWaiterId { get; set; }
    }

    public class UpdateOrderItemRequest
    {
        [Required]
        [Range(1, 50)]
        public int Quantity { get; set; }

        [StringLength(200)]
        public string? SpecialInstructions { get; set; }

        [Required]
        public int Status { get; set; }
    }
}

