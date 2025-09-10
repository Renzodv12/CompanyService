using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.DynamicReports
{
    public class CreateReportDefinitionDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [StringLength(100)]
        public string EntityName { get; set; } = string.Empty;

        [Required]
        public Guid CompanyId { get; set; }

        public bool IsShared { get; set; } = false;

        public List<CreateReportFieldDto> Fields { get; set; } = new();
        public List<CreateReportFilterDto> Filters { get; set; } = new();
    }

    public class CreateReportFieldDto
    {
        [Required]
        [StringLength(100)]
        public string FieldName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        public ReportDataType DataType { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsVisible { get; set; } = true;
        public AggregateFunction? AggregateFunction { get; set; }
        
        [StringLength(50)]
        public string? FormatString { get; set; }
    }

    public class CreateReportFilterDto
    {
        [Required]
        [StringLength(100)]
        public string FieldName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        public ReportDataType DataType { get; set; }

        [Required]
        public FilterOperator Operator { get; set; }

        [StringLength(500)]
        public string? DefaultValue { get; set; }

        public bool IsRequired { get; set; } = false;
        public bool IsUserEditable { get; set; } = true;
        public int DisplayOrder { get; set; }
    }
}