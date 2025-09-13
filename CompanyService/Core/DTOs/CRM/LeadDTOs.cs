using CompanyService.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.CRM
{
    public class CreateLeadDto
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

        [StringLength(100)]
        public string? Company { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        public LeadSource Source { get; set; }

        public LeadStatus Status { get; set; } = LeadStatus.New;

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime? NextFollowUpDate { get; set; }

        public bool IsQualified { get; set; } = false;

        [Required]
        public Guid CompanyId { get; set; }

        public Guid? AssignedUserId { get; set; }
    }

    public class UpdateLeadDto
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Company { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        public LeadSource? Source { get; set; }

        public LeadStatus? Status { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime? NextFollowUpDate { get; set; }

        public bool? IsQualified { get; set; }

        public Guid? AssignedUserId { get; set; }
    }

    public class LeadDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? JobTitle { get; set; }
        public LeadSource Source { get; set; }
        public LeadStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public bool IsQualified { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? AssignedUserName { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }

    public class LeadListDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Company { get; set; }
        public LeadSource Source { get; set; }
        public LeadStatus Status { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public bool IsQualified { get; set; }
        public string? AssignedUserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}