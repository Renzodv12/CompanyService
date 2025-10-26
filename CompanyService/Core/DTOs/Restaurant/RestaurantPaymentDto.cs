using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.Restaurant
{
    public class RestaurantPaymentDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public string? Notes { get; set; }
        public DateTime PaymentTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public Guid ProcessedBy { get; set; }
        public string ProcessedByName { get; set; } = string.Empty;
    }

    public class CreateRestaurantPaymentRequest
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        public int Method { get; set; }
        
        [StringLength(100)]
        public string? TransactionId { get; set; }
        
        [StringLength(200)]
        public string? Notes { get; set; }
    }

    public class UpdateRestaurantPaymentRequest
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        public int Method { get; set; }
        
        [Required]
        public int Status { get; set; }
        
        [StringLength(100)]
        public string? TransactionId { get; set; }
        
        [StringLength(200)]
        public string? Notes { get; set; }
    }

    public class RestaurantDashboardDto
    {
        public int TotalTables { get; set; }
        public int AvailableTables { get; set; }
        public int OccupiedTables { get; set; }
        public int ReservedTables { get; set; }
        public int ActiveOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrdersToday { get; set; }
        public decimal TotalRevenueToday { get; set; }
        public decimal TotalRevenueThisMonth { get; set; }
        public List<RestaurantOrderDto> RecentOrders { get; set; } = new();
        public List<RestaurantTableDto> TableStatus { get; set; } = new();
    }
}

