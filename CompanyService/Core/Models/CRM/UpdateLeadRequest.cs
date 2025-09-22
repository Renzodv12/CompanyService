using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Models.CRM
{
    public class UpdateLeadRequest
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? CompanyName { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        [Required]
        public LeadSource Source { get; set; }

        [Required]
        public LeadStatus Status { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime? NextFollowUpDate { get; set; }

        public Guid? AssignedToUserId { get; set; }

        public bool IsQualified { get; set; }
    }
}