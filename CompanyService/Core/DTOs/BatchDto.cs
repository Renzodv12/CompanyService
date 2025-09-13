using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs
{
    public class BatchDto
    {
        public Guid Id { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal InitialQuantity { get; set; }
        public decimal? UnitCost { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? Supplier { get; set; }
        public string? LotNumber { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateBatchDto
    {
        [Required]
        [StringLength(50)]
        public string BatchNumber { get; set; } = string.Empty;
        
        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        public Guid WarehouseId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than 0")]
        public decimal? UnitCost { get; set; }
        
        public DateTime? ManufactureDate { get; set; }
        
        public DateTime? ExpirationDate { get; set; }
        
        [StringLength(100)]
        public string? Supplier { get; set; }
        
        [StringLength(50)]
        public string? LotNumber { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        public Guid CompanyId { get; set; }
    }

    public class UpdateBatchDto
    {
        [Required]
        [StringLength(50)]
        public string BatchNumber { get; set; } = string.Empty;
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 0")]
        public decimal Quantity { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than 0")]
        public decimal? UnitCost { get; set; }
        
        public DateTime? ManufactureDate { get; set; }
        
        public DateTime? ExpirationDate { get; set; }
        
        [StringLength(100)]
        public string? Supplier { get; set; }
        
        [StringLength(50)]
        public string? LotNumber { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public bool IsActive { get; set; }
    }

    public class BatchResponseDto
    {
        public Guid Id { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal InitialQuantity { get; set; }
        public decimal UsedQuantity => InitialQuantity - Quantity;
        public decimal? UnitCost { get; set; }
        public decimal? TotalValue => UnitCost * Quantity;
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? DaysToExpiration => ExpirationDate?.Subtract(DateTime.UtcNow).Days;
        public bool IsExpired => ExpirationDate.HasValue && ExpirationDate < DateTime.UtcNow;
        public bool IsNearExpiration => DaysToExpiration.HasValue && DaysToExpiration <= 30;
        public string? Supplier { get; set; }
        public string? LotNumber { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class BatchMovementDto
    {
        [Required]
        public Guid BatchId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }
        
        [Required]
        [StringLength(20)]
        public string MovementType { get; set; } = string.Empty; // "IN" or "OUT"
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}