using CompanyService.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.CRM
{
    public class CreateCampaignDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public CampaignType Type { get; set; }

        public CampaignStatus Status { get; set; } = CampaignStatus.Draft;

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Budget { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ActualCost { get; set; } = 0;

        public int TargetAudience { get; set; } = 0;

        public int ActualReach { get; set; } = 0;

        [Range(0, 100)]
        public decimal ConversionRate { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal ROI { get; set; } = 0;

        [StringLength(100)]
        public string? Channel { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        public Guid? AssignedUserId { get; set; }
    }

    public class UpdateCampaignDto
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public CampaignType? Type { get; set; }

        public CampaignStatus? Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Budget { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ActualCost { get; set; }

        public int? TargetAudience { get; set; }

        public int? ActualReach { get; set; }

        [Range(0, 100)]
        public decimal? ConversionRate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ROI { get; set; }

        [StringLength(100)]
        public string? Channel { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public Guid? AssignedUserId { get; set; }
    }

    public class CampaignDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public CampaignType Type { get; set; }
        public CampaignStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
        public int TargetAudience { get; set; }
        public int ActualReach { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal ROI { get; set; }
        public string? Channel { get; set; }
        public string? Notes { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? AssignedUserName { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int LeadsCount { get; set; }
    }

    public class CampaignListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CampaignType Type { get; set; }
        public CampaignStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
        public int TargetAudience { get; set; }
        public int ActualReach { get; set; }
        public decimal ROI { get; set; }
        public string? AssignedUserName { get; set; }
        public int LeadsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CampaignStatsDto
    {
        public Guid CampaignId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public int TotalLeads { get; set; }
        public int QualifiedLeads { get; set; }
        public int ConvertedLeads { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal ROI { get; set; }
        public decimal CostPerLead { get; set; }
        public decimal Revenue { get; set; }
    }
}