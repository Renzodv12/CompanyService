using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class GoodsReceiptItem
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid GoodsReceiptId { get; set; }
        
        [Required]
        public Guid PurchaseOrderItemId { get; set; }
        
        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal OrderedQuantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal ReceivedQuantity { get; set; }
        
        [Column(TypeName = "decimal(18,4)")]
        public decimal AcceptedQuantity { get; set; }
        
        [Column(TypeName = "decimal(18,4)")]
        public decimal RejectedQuantity { get; set; }
        
        [Column(TypeName = "decimal(18,4)")]
        public decimal DamagedQuantity { get; set; }
        
        [Required]
        public QualityStatus QualityStatus { get; set; }
        
        [StringLength(500)]
        public string? QualityNotes { get; set; }
        
        [StringLength(100)]
        public string? BatchNumber { get; set; }
        
        [StringLength(100)]
        public string? SerialNumber { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        
        public DateTime? ManufactureDate { get; set; }
        
        [StringLength(100)]
        public string? StorageLocation { get; set; }
        
        public Guid? WarehouseId { get; set; }
        
        [Required]
        public bool RequiresInspection { get; set; } = false;
        
        [Required]
        public bool IsInspected { get; set; } = false;
        
        public DateTime? InspectionDate { get; set; }
        
        public Guid? InspectedBy { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        // Navigation Properties
        [ForeignKey("GoodsReceiptId")]
        public virtual GoodsReceipt GoodsReceipt { get; set; } = null!;
        
        [ForeignKey("PurchaseOrderItemId")]
        public virtual PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
        
        [ForeignKey("WarehouseId")]
        public virtual Warehouse? Warehouse { get; set; }
        
        [ForeignKey("InspectedBy")]
        public virtual User? InspectedByUser { get; set; }
    }
}