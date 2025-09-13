using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class CustomerTracking
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        public Guid CustomerId { get; set; }
        
        [Required]
        public CustomerInteractionType InteractionType { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public DateTime InteractionDate { get; set; } = DateTime.UtcNow;
        
        public Guid? UserId { get; set; }
        
        [StringLength(100)]
        public string? Channel { get; set; }
        
        public CustomerSatisfactionLevel? SatisfactionLevel { get; set; }
        
        [StringLength(500)]
        public string? Outcome { get; set; }
        
        [StringLength(300)]
        public string? NextAction { get; set; }
        
        public DateTime? NextActionDate { get; set; }
        
        public bool IsFollowUpRequired { get; set; } = false;
        
        public DateTime? FollowUpDate { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? RelatedAmount { get; set; }
        
        public Guid? RelatedOpportunityId { get; set; }
        
        public Guid? RelatedSaleId { get; set; }
        
        [StringLength(200)]
        public string? Tags { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public string? ModifiedBy { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
        
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;
        
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        
        [ForeignKey("RelatedOpportunityId")]
        public virtual Opportunity? RelatedOpportunity { get; set; }
        
        [ForeignKey("RelatedSaleId")]
        public virtual Sale? RelatedSale { get; set; }
    }
}