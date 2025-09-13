using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Entities;

namespace CompanyService.Core.DTOs
{
    public class ReorderPointDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public decimal MinimumQuantity { get; set; }
        public decimal ReorderQuantity { get; set; }
        public decimal? SafetyStock { get; set; }
        public int LeadTimeDays { get; set; }
        public decimal? AverageDailyUsage { get; set; }
        public ReorderStatus Status { get; set; }
        public DateTime? LastTriggeredDate { get; set; }
        public DateTime? LastOrderedDate { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal ReorderLevel { get; set; }
    }

    public class CreateReorderPointDto
    {
        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        public Guid WarehouseId { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum quantity must be greater than or equal to 0")]
        public decimal MinimumQuantity { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Reorder quantity must be greater than 0")]
        public decimal ReorderQuantity { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Safety stock must be greater than or equal to 0")]
        public decimal? SafetyStock { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Lead time days must be greater than 0")]
        public int LeadTimeDays { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Average daily usage must be greater than or equal to 0")]
        public decimal? AverageDailyUsage { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        public Guid CompanyId { get; set; }
    }

    public class UpdateReorderPointDto
    {
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum quantity must be greater than or equal to 0")]
        public decimal MinimumQuantity { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Reorder quantity must be greater than 0")]
        public decimal ReorderQuantity { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Safety stock must be greater than or equal to 0")]
        public decimal? SafetyStock { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Lead time days must be greater than 0")]
        public int LeadTimeDays { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Average daily usage must be greater than or equal to 0")]
        public decimal? AverageDailyUsage { get; set; }
        
        [Required]
        public ReorderStatus Status { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public bool IsActive { get; set; }
    }

    public class ReorderPointResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public decimal MinimumQuantity { get; set; }
        public decimal ReorderQuantity { get; set; }
        public decimal? SafetyStock { get; set; }
        public int LeadTimeDays { get; set; }
        public decimal? AverageDailyUsage { get; set; }
        public ReorderStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public DateTime? LastTriggeredDate { get; set; }
        public DateTime? LastOrderedDate { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal? CurrentStock { get; set; }
        public bool ShouldReorder => CurrentStock.HasValue && CurrentStock <= ReorderLevel;
        public int? DaysSinceLastTriggered => LastTriggeredDate?.Subtract(DateTime.UtcNow).Days;
    }

    public class ReorderAlertDto
    {
        public Guid Id { get; set; }
        public Guid ReorderPointId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public decimal CurrentQuantity { get; set; }
        public decimal TriggeredQuantity { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal ReorderQuantity { get; set; }
        public DateTime AlertDate { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string? Notes { get; set; }
        public int DaysOpen => IsResolved && ResolvedDate.HasValue ? 
            (int)(ResolvedDate.Value - AlertDate).TotalDays : 
            (int)(DateTime.UtcNow - AlertDate).TotalDays;
    }

    public class ResolveReorderAlertDto
    {
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}