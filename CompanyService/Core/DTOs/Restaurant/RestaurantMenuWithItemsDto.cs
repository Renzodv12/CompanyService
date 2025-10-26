using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.Restaurant
{
    public class RestaurantMenuWithItemsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public int AvailableItems { get; set; }
        public List<RestaurantMenuItemDto> Items { get; set; } = new();
    }

    public class TopSellingItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RevenueByDayDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class RestaurantReportsDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<TopSellingItemDto> TopSellingItems { get; set; } = new();
        public List<RevenueByDayDto> RevenueByDay { get; set; } = new();
    }

    public class PeriodStatsDto
    {
        public int Orders { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class RestaurantStatsDto
    {
        public PeriodStatsDto Today { get; set; } = new();
        public PeriodStatsDto ThisWeek { get; set; } = new();
        public PeriodStatsDto ThisMonth { get; set; } = new();
        public PeriodStatsDto AllTime { get; set; } = new();
    }

    public class TableStatusDto
    {
        public Guid Id { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Location { get; set; }
        public bool IsActive { get; set; }
        public Guid? CurrentOrderId { get; set; }
        public string? CurrentOrderNumber { get; set; }
        public int? CurrentGuests { get; set; }
        public DateTime? OrderStartTime { get; set; }
        public DateTime? EstimatedCompletionTime { get; set; }
    }
}

