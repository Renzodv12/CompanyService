using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    public class Batch
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string BatchNumber { get; set; } = string.Empty;
        
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
        public decimal Quantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal InitialQuantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? UnitCost { get; set; }
        
        public DateTime? ManufactureDate { get; set; }
        
        public DateTime? ExpirationDate { get; set; }
        
        [StringLength(100)]
        public string? Supplier { get; set; }
        
        [StringLength(50)]
        public string? LotNumber { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
    }
}