using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class Campaign
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
        
        [Required]
        public CampaignType Type { get; set; } = CampaignType.Email;
        
        [Required]
        public CampaignStatus Status { get; set; } = CampaignStatus.Draft;
        
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Budget { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualCost { get; set; } = 0;
        
        public int TargetAudience { get; set; } = 0;
        
        public int ActualReach { get; set; } = 0;
        
        public int Leads { get; set; } = 0;
        
        public int Conversions { get; set; } = 0;
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal ConversionRate { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Revenue { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal ROI { get; set; } = 0;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public Guid? AssignedToUserId { get; set; }
        
        [StringLength(200)]
        public string? Channel { get; set; }
        
        [StringLength(300)]
        public string? Message { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public string? ModifiedBy { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
        
        [ForeignKey("AssignedToUserId")]
        public virtual User? AssignedToUser { get; set; }
        
        public virtual ICollection<CampaignLead> CampaignLeads { get; set; } = new List<CampaignLead>();
    }
}