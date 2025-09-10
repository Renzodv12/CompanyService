using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.DynamicReports
{
    public class ReportExecutionResponseDto
    {
        public Guid Id { get; set; }
        public Guid ReportDefinitionId { get; set; }
        public string ReportDefinitionName { get; set; } = string.Empty;
        public Guid ExecutedByUserId { get; set; }
        public string ExecutedByUserName { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public ReportExecutionStatus Status { get; set; }
        public string? FiltersApplied { get; set; }
        public int RowCount { get; set; }
        public TimeSpan? ExecutionDuration { get; set; }
        public string? ErrorMessage { get; set; }
        public ExportFormat? ExportFormat { get; set; }
        public string? ExportFileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public DateTime ExecutedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<Dictionary<string, object?>> Data { get; set; } = new();
        public ReportMetadataDto Metadata { get; set; } = new();
    }

    public class ReportMetadataDto
    {
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public List<ReportColumnMetadataDto> Columns { get; set; } = new();
    }

    public class ReportColumnMetadataDto
    {
        public string FieldName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public ReportDataType DataType { get; set; }
        public AggregateFunction? AggregateFunction { get; set; }
        public string? FormatString { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class ReportExecutionListDto
    {
        public Guid Id { get; set; }
        public string ReportDefinitionName { get; set; } = string.Empty;
        public string ExecutedByUserName { get; set; } = string.Empty;
        public ReportExecutionStatus Status { get; set; }
        public int RowCount { get; set; }
        public TimeSpan? ExecutionDuration { get; set; }
        public ExportFormat? ExportFormat { get; set; }
        public string? ExportFileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public DateTime ExecutedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    }

    public enum ReportExecutionStatus
    {
        Pending = 0,
        Running = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4
    }
}