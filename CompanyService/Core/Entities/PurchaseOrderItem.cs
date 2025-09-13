using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    public class PurchaseOrderItem
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid PurchaseOrderId { get; set; }
        
        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Quantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }
        
        [Column(TypeName = "decimal(18,4)")]
        public decimal ReceivedQuantity { get; set; }
        
        [Column(TypeName = "decimal(18,4)")]
        public decimal RemainingQuantity { get; set; }
        
        [StringLength(200)]
        public string? Description { get; set; }
        
        [StringLength(50)]
        public string? Unit { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        // Navigation Properties
        [ForeignKey("PurchaseOrderId")]
        public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
        
        public virtual ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; } = new List<GoodsReceiptItem>();
    }
}