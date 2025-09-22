using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.DynamicReports
{
    public class ReportDefinitionResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsShared { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;

        public List<ReportFieldResponseDto> Fields { get; set; } = new();
        public List<ReportFilterResponseDto> Filters { get; set; } = new();
    }

    public class ReportFieldResponseDto
    {
        public Guid Id { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public ReportDataType DataType { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsVisible { get; set; }
        public AggregateFunction? AggregateFunction { get; set; }
        public string? FormatString { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReportFilterResponseDto
    {
        public Guid Id { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public ReportDataType DataType { get; set; }
        public FilterOperator Operator { get; set; }
        public string? DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public bool IsUserEditable { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}