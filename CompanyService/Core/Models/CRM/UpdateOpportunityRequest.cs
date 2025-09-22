using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Models.CRM
{
    public class UpdateOpportunityRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public OpportunityStage Stage { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Value { get; set; }

        [Range(0, 100)]
        public int Probability { get; set; }

        [Required]
        public DateTime ExpectedCloseDate { get; set; }

        public DateTime? ActualCloseDate { get; set; }

        public Guid? LeadId { get; set; }

        public Guid? AssignedToUserId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}