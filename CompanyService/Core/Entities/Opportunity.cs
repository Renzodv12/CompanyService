using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class Opportunity
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public Guid? LeadId { get; set; }
        
        public Guid? CustomerId { get; set; }
        
        [Required]
        public OpportunityStage Stage { get; set; } = OpportunityStage.Prospecting;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal EstimatedValue { get; set; }
        
        [Range(0, 100)]
        public int Probability { get; set; } = 0;
        
        public DateTime? ExpectedCloseDate { get; set; }
        
        public DateTime? ActualCloseDate { get; set; }
        
        public Guid? AssignedToUserId { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [StringLength(100)]
        public string? CompetitorInfo { get; set; }
        
        [StringLength(200)]
        public string? NextAction { get; set; }
        
        public DateTime? NextActionDate { get; set; }
        
        public bool IsWon { get; set; } = false;
        
        public bool IsLost { get; set; } = false;
        
        [StringLength(300)]
        public string? LostReason { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public string? ModifiedBy { get; set; }
        
        public DateTime? CloseDate { get; set; }
        
        public decimal ActualValue { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
        
        [ForeignKey("LeadId")]
        public virtual Lead? Lead { get; set; }
        
        [ForeignKey("AssignedToUserId")]
        public virtual User? AssignedUser { get; set; }
        
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
    }
}