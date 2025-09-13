using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Entities;

namespace CompanyService.Core.DTOs
{
    public class PhysicalInventoryDto
    {
        public Guid Id { get; set; }
        public string InventoryNumber { get; set; } = string.Empty;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public PhysicalInventoryStatus Status { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalItems { get; set; }
        public int CountedItems { get; set; }
        public decimal CompletionPercentage => TotalItems > 0 ? (decimal)CountedItems / TotalItems * 100 : 0;
    }

    public class CreatePhysicalInventoryDto
    {
        [Required]
        [StringLength(50)]
        public string InventoryNumber { get; set; } = string.Empty;
        
        [Required]
        public Guid WarehouseId { get; set; }
        
        [Required]
        public DateTime ScheduledDate { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        public Guid CompanyId { get; set; }
        
        public List<CreatePhysicalInventoryItemDto> Items { get; set; } = new List<CreatePhysicalInventoryItemDto>();
    }

    public class UpdatePhysicalInventoryDto
    {
        [Required]
        [StringLength(50)]
        public string InventoryNumber { get; set; } = string.Empty;
        
        [Required]
        public DateTime ScheduledDate { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? CompletedDate { get; set; }
        
        [Required]
        public PhysicalInventoryStatus Status { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class PhysicalInventoryResponseDto
    {
        public Guid Id { get; set; }
        public string InventoryNumber { get; set; } = string.Empty;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public PhysicalInventoryStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalItems { get; set; }
        public int CountedItems { get; set; }
        public decimal CompletionPercentage => TotalItems > 0 ? (decimal)CountedItems / TotalItems * 100 : 0;
        public decimal TotalVariance { get; set; }
        public List<PhysicalInventoryItemResponseDto> Items { get; set; } = new List<PhysicalInventoryItemResponseDto>();
    }

    public class PhysicalInventoryItemDto
    {
        public Guid Id { get; set; }
        public Guid PhysicalInventoryId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid? BatchId { get; set; }
        public string? BatchNumber { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal? CountedQuantity { get; set; }
        public decimal? Variance { get; set; }
        public string? Notes { get; set; }
        public bool IsCounted { get; set; }
        public DateTime? CountedAt { get; set; }
        public string? CountedBy { get; set; }
    }

    public class CreatePhysicalInventoryItemDto
    {
        [Required]
        public Guid ProductId { get; set; }
        
        public Guid? BatchId { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "System quantity must be greater than or equal to 0")]
        public decimal SystemQuantity { get; set; }
    }

    public class UpdatePhysicalInventoryItemDto
    {
        [Range(0, double.MaxValue, ErrorMessage = "Counted quantity must be greater than or equal to 0")]
        public decimal? CountedQuantity { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [StringLength(100)]
        public string? CountedBy { get; set; }
    }

    public class PhysicalInventoryItemResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public Guid? BatchId { get; set; }
        public string? BatchNumber { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal? CountedQuantity { get; set; }
        public decimal? Variance => CountedQuantity.HasValue ? CountedQuantity - SystemQuantity : null;
        public decimal? VariancePercentage => SystemQuantity > 0 && Variance.HasValue ? (Variance / SystemQuantity) * 100 : null;
        public string? Notes { get; set; }
        public bool IsCounted { get; set; }
        public DateTime? CountedAt { get; set; }
        public string? CountedBy { get; set; }
    }
}