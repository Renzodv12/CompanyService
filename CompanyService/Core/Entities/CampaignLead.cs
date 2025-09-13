using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class CampaignLead
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid CampaignId { get; set; }
        
        [Required]
        public Guid LeadId { get; set; }
        
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        
        public CampaignLeadStatus Status { get; set; } = CampaignLeadStatus.Added;
        
        public DateTime? ContactedDate { get; set; }
        
        public DateTime? RespondedDate { get; set; }
        
        public bool IsConverted { get; set; } = false;
        
        [StringLength(300)]
        public string? Response { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        // Navigation properties
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; } = null!;
        
        [ForeignKey("LeadId")]
        public virtual Lead Lead { get; set; } = null!;
    }
}