using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.Finance
{
    public class ChartOfAccountsDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public string? ParentName { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ChartOfAccountsDto> Children { get; set; } = new();
    }

    public class CreateChartOfAccountsRequest
    {
        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        public Guid? ParentId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class UpdateChartOfAccountsRequest
    {
        [StringLength(20)]
        public string? Code { get; set; }

        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(50)]
        public string? Type { get; set; }

        public Guid? ParentId { get; set; }

        public bool? IsActive { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
