using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    public enum ReorderStatus
    {
        Active,
        Triggered,
        Ordered,
        Inactive
    }

    public class ReorderPoint
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid ProductId { get; set; }
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
        
        [Required]
        public Guid WarehouseId { get; set; }
        
        [ForeignKey("WarehouseId")]
        public virtual Warehouse Warehouse { get; set; } = null!;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumQuantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ReorderQuantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SafetyStock { get; set; }
        
        [Required]
        public int LeadTimeDays { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AverageDailyUsage { get; set; }
        
        [Required]
        public ReorderStatus Status { get; set; } = ReorderStatus.Active;
        
        public DateTime? LastTriggeredDate { get; set; }
        
        public DateTime? LastOrderedDate { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Calculated property
        [NotMapped]
        public decimal ReorderLevel => MinimumQuantity + (SafetyStock ?? 0);
        
        // Navigation properties
        public virtual ICollection<ReorderAlert> ReorderAlerts { get; set; } = new List<ReorderAlert>();
    }

    public class ReorderAlert
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid ReorderPointId { get; set; }
        
        [ForeignKey("ReorderPointId")]
        public virtual ReorderPoint ReorderPoint { get; set; } = null!;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentQuantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TriggeredQuantity { get; set; }
        
        [Required]
        public DateTime AlertDate { get; set; } = DateTime.UtcNow;
        
        public bool IsResolved { get; set; } = false;
        
        public DateTime? ResolvedDate { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}