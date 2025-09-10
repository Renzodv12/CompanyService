using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.DynamicReports
{
    public class UpdateReportDefinitionDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsShared { get; set; } = false;

        public List<UpdateReportFieldDto> Fields { get; set; } = new();
        public List<UpdateReportFilterDto> Filters { get; set; } = new();
    }

    public class UpdateReportFieldDto
    {
        public Guid? Id { get; set; }

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

    public class UpdateReportFilterDto
    {
        public Guid? Id { get; set; }

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