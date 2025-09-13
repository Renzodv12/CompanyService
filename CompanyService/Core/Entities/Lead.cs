using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class Lead
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(200)]
        public string? CompanyName { get; set; }
        
        [StringLength(100)]
        public string? JobTitle { get; set; }
        
        [Required]
        public LeadStatus Status { get; set; } = LeadStatus.New;
        
        [Required]
        public LeadSource Source { get; set; } = LeadSource.Website;
        
        public int Score { get; set; } = 0;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public Guid? AssignedToUserId { get; set; }
        
        public DateTime? LastContactDate { get; set; }
        
        public DateTime? NextFollowUpDate { get; set; }
        
        public bool IsQualified { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public string? ModifiedBy { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
        
        [ForeignKey("AssignedToUserId")]
        public virtual User? AssignedUser { get; set; }
        
        public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
        
        public virtual ICollection<CampaignLead> CampaignLeads { get; set; } = new List<CampaignLead>();
    }
}