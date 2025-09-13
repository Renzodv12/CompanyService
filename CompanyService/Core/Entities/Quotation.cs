using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class Quotation
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string QuotationNumber { get; set; } = string.Empty;
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        public Guid SupplierId { get; set; }
        
        [Required]
        public DateTime QuotationDate { get; set; }
        
        public DateTime? ValidUntil { get; set; }
        
        [Required]
        public QuotationStatus Status { get; set; }
        
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
        
        [StringLength(50)]
        public string? PaymentTerms { get; set; }
        
        [StringLength(50)]
        public string? DeliveryTerms { get; set; }
        
        public int? DeliveryDays { get; set; }
        
        [StringLength(100)]
        public string? Currency { get; set; }
        
        public Guid? ConvertedToPurchaseOrderId { get; set; }
        
        public DateTime? ConvertedDate { get; set; }
        
        public Guid RequestedBy { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public Guid? ModifiedBy { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        // Navigation Properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
        
        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; } = null!;
        
        [ForeignKey("RequestedBy")]
        public virtual User RequestedByUser { get; set; } = null!;
        
        [ForeignKey("ModifiedBy")]
        public virtual User? ModifiedByUser { get; set; }
        
        [ForeignKey("ConvertedToPurchaseOrderId")]
        public virtual PurchaseOrder? ConvertedToPurchaseOrder { get; set; }
        
        public virtual ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
    }
}