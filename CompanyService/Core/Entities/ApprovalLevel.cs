using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    public class ApprovalLevel
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Description { get; set; }
        
        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } = string.Empty; // PurchaseOrder, Quotation, etc.
        
        [Required]
        public int Level { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxAmount { get; set; }
        
        [Required]
        public bool RequiresApproval { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        public int? TimeoutHours { get; set; }
        
        [Required]
        public bool AllowDelegation { get; set; } = false;
        
        [Required]
        public bool RequireAllApprovers { get; set; } = false; // true = all approvers must approve, false = any approver can approve
        
        [Required]
        public int RequiredApprovals { get; set; } = 1; // Number of required approvals
        
        public DateTime CreatedDate { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public Guid? ModifiedBy { get; set; }
        
        // Additional properties for compatibility
        public bool RequiresAllApprovers { get; set; } = false;
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime LastModifiedAt { get; set; }
        
        // Navigation Properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
        
        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } = null!;
        
        [ForeignKey("ModifiedBy")]
        public virtual User? ModifiedByUser { get; set; }
        
        public virtual ICollection<ApprovalLevelUser> ApprovalLevelUsers { get; set; } = new List<ApprovalLevelUser>();
        
        public virtual ICollection<Approval> Approvals { get; set; } = new List<Approval>();
    }
}