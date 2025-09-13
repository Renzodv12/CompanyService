using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    public class ApprovalLevelUser
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid ApprovalLevelId { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        [Required]
        public bool IsPrimary { get; set; } = false;
        
        public Guid? DelegateUserId { get; set; }
        
        public DateTime? DelegateFromDate { get; set; }
        
        public DateTime? DelegateToDate { get; set; }
        
        [StringLength(200)]
        public string? DelegateReason { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public Guid? ModifiedBy { get; set; }
        
        // Navigation Properties
        [ForeignKey("ApprovalLevelId")]
        public virtual ApprovalLevel ApprovalLevel { get; set; } = null!;
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        
        [ForeignKey("DelegateUserId")]
        public virtual User? DelegateUser { get; set; }
        
        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } = null!;
        
        [ForeignKey("ModifiedBy")]
        public virtual User? ModifiedByUser { get; set; }
    }
}