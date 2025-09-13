using CompanyService.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.CRM
{
    public class CreateOpportunityDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Value { get; set; }

        public OpportunityStage Stage { get; set; } = OpportunityStage.Prospecting;

        [Range(0, 100)]
        public int Probability { get; set; } = 0;

        public DateTime? ExpectedCloseDate { get; set; }

        public DateTime? ActualCloseDate { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        public Guid? AssignedUserId { get; set; }

        public Guid? LeadId { get; set; }
    }

    public class UpdateOpportunityDto
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Value { get; set; }

        public OpportunityStage? Stage { get; set; }

        [Range(0, 100)]
        public int? Probability { get; set; }

        public DateTime? ExpectedCloseDate { get; set; }

        public DateTime? ActualCloseDate { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public Guid? AssignedUserId { get; set; }

        public Guid? LeadId { get; set; }
    }

    public class OpportunityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Value { get; set; }
        public OpportunityStage Stage { get; set; }
        public int Probability { get; set; }
        public DateTime? ExpectedCloseDate { get; set; }
        public DateTime? ActualCloseDate { get; set; }
        public string? Notes { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public Guid? LeadId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? AssignedUserName { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? LeadName { get; set; }
    }

    public class OpportunityListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public OpportunityStage Stage { get; set; }
        public int Probability { get; set; }
        public DateTime? ExpectedCloseDate { get; set; }
        public string? AssignedUserName { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class OpportunityStageUpdateDto
    {
        [Required]
        public OpportunityStage Stage { get; set; }

        [Range(0, 100)]
        public int Probability { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime? ActualCloseDate { get; set; }
    }
}