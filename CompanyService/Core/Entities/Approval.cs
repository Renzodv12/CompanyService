using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class Approval
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } = string.Empty; // PurchaseOrder, Quotation, etc.
        
        [Required]
        public Guid DocumentId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string DocumentNumber { get; set; } = string.Empty;
        
        [Required]
        public Guid RequestedBy { get; set; }
        
        [Required]
        public DateTime RequestedDate { get; set; }
        
        [Required]
        public Guid ApprovalLevelId { get; set; }
        
        [Required]
        public int ApprovalOrder { get; set; }
        
        [Required]
        public Guid ApproverId { get; set; }
        
        [Required]
        public ApprovalStatus Status { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DocumentAmount { get; set; }
        
        public DateTime? ApprovedDate { get; set; }
        
        public DateTime? RejectedDate { get; set; }
        
        [StringLength(500)]
        public string? Comments { get; set; }
        
        [StringLength(500)]
        public string? RejectionReason { get; set; }
        
        public DateTime? DueDate { get; set; }
        
        public DateTime? RequiredDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; }
        
        public string? CreatedBy { get; set; }
        
        public string? ModifiedBy { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        // Additional properties for compatibility
        public DateTime RequestDate { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime LastModifiedAt { get; set; }
        
        // Navigation Properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
        
        [ForeignKey("RequestedBy")]
        public virtual User RequestedByUser { get; set; } = null!;
        
        [ForeignKey("ApproverId")]
        public virtual User ApproverUser { get; set; } = null!;
        
        [ForeignKey("ApprovalLevelId")]
        public virtual ApprovalLevel ApprovalLevel { get; set; } = null!;
    }
}