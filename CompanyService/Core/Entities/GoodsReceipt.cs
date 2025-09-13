using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class GoodsReceipt
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ReceiptNumber { get; set; } = string.Empty;
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        public Guid PurchaseOrderId { get; set; }
        
        [Required]
        public Guid SupplierId { get; set; }
        
        [Required]
        public DateTime ReceiptDate { get; set; }
        
        [Required]
        public GoodsReceiptStatus Status { get; set; }
        
        [StringLength(100)]
        public string? DeliveryNote { get; set; }
        
        [StringLength(100)]
        public string? InvoiceNumber { get; set; }
        
        [StringLength(100)]
        public string? TransportCompany { get; set; }
        
        [StringLength(100)]
        public string? VehicleNumber { get; set; }
        
        [StringLength(100)]
        public string? DriverName { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [StringLength(500)]
        public string? QualityNotes { get; set; }
        
        [Required]
        public bool IsPartialReceipt { get; set; } = false;
        
        [Required]
        public bool IsCompleted { get; set; } = false;
        
        public Guid ReceivedBy { get; set; }
        
        public Guid? InspectedBy { get; set; }
        
        public DateTime? InspectionDate { get; set; }
        
        public Guid? ApprovedBy { get; set; }
        
        public DateTime? ApprovedDate { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        // Navigation Properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
        
        [ForeignKey("PurchaseOrderId")]
        public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
        
        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; } = null!;
        
        [ForeignKey("ReceivedBy")]
        public virtual User ReceivedByUser { get; set; } = null!;
        
        [ForeignKey("InspectedBy")]
        public virtual User? InspectedByUser { get; set; }
        
        [ForeignKey("ApprovedBy")]
        public virtual User? ApprovedByUser { get; set; }
        
        public virtual ICollection<GoodsReceiptItem> Items { get; set; } = new List<GoodsReceiptItem>();
    }
}