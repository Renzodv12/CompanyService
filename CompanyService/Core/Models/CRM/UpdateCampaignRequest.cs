using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Models.CRM
{
    public class UpdateCampaignRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public CampaignType Type { get; set; }

        [Required]
        public CampaignStatus Status { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Budget { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ActualCost { get; set; }

        [Range(0, int.MaxValue)]
        public int ExpectedRevenue { get; set; }

        [Range(0, int.MaxValue)]
        public int ActualRevenue { get; set; }

        public Guid? AssignedToUserId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}