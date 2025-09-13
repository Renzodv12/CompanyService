using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class PurchaseOrder
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        public Guid SupplierId { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; }
        
        public DateTime? ExpectedDeliveryDate { get; set; }
        
        public DateTime? ActualDeliveryDate { get; set; }
        
        [Required]
        public PurchaseOrderStatus Status { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [StringLength(200)]
        public string? DeliveryAddress { get; set; }
        
        [StringLength(50)]
        public string? PaymentTerms { get; set; }
        
        [StringLength(200)]
        public string? DeliveryTerms { get; set; }
        
        [StringLength(10)]
        public string? Currency { get; set; }
        
        [StringLength(500)]
        public string? CancellationReason { get; set; }
        
        public DateTime? SentDate { get; set; }
        
        public DateTime? CancelledDate { get; set; }
        
        public DateTime? RejectedDate { get; set; }
        
        [StringLength(500)]
        public string? RejectionReason { get; set; }
        
        public Guid? RejectedBy { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public Guid? ApprovedBy { get; set; }
        
        public DateTime? ApprovedDate { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public Guid? ModifiedBy { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        // Navigation Properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
        
        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; } = null!;
        
        [ForeignKey("ApprovedBy")]
        public virtual User? ApprovedByUser { get; set; }
        
        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } = null!;
        
        [ForeignKey("ModifiedBy")]
        public virtual User? ModifiedByUser { get; set; }
        
        public virtual ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
        
        public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();
    }
}