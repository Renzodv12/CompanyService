using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    public class QuotationItem
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid QuotationId { get; set; }
        
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
        
        [StringLength(200)]
        public string? Description { get; set; }
        
        [StringLength(50)]
        public string? Unit { get; set; }
        
        [StringLength(100)]
        public string? Brand { get; set; }
        
        [StringLength(100)]
        public string? Model { get; set; }
        
        [StringLength(500)]
        public string? Specifications { get; set; }
        
        public int? LeadTimeDays { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        // Navigation Properties
        [ForeignKey("QuotationId")]
        public virtual Quotation Quotation { get; set; } = null!;
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}