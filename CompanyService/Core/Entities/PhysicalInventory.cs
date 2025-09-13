using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    public enum PhysicalInventoryStatus
    {
        Planned,
        InProgress,
        Completed,
        Cancelled
    }

    public class PhysicalInventory
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string InventoryNumber { get; set; } = string.Empty;
        
        [Required]
        public Guid WarehouseId { get; set; }
        
        [ForeignKey("WarehouseId")]
        public virtual Warehouse Warehouse { get; set; } = null!;
        
        [Required]
        public DateTime ScheduledDate { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? CompletedDate { get; set; }
        
        [Required]
        public PhysicalInventoryStatus Status { get; set; } = PhysicalInventoryStatus.Planned;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual ICollection<PhysicalInventoryItem> Items { get; set; } = new List<PhysicalInventoryItem>();
    }

    public class PhysicalInventoryItem
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid PhysicalInventoryId { get; set; }
        
        [ForeignKey("PhysicalInventoryId")]
        public virtual PhysicalInventory PhysicalInventory { get; set; } = null!;
        
        [Required]
        public Guid ProductId { get; set; }
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
        
        public Guid? BatchId { get; set; }
        
        [ForeignKey("BatchId")]
        public virtual Batch? Batch { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SystemQuantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CountedQuantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Variance { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public bool IsCounted { get; set; } = false;
        
        public DateTime? CountedAt { get; set; }
        
        [StringLength(100)]
        public string? CountedBy { get; set; }
    }
}