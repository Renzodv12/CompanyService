using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.DynamicReports
{
    public class ExecuteReportDto
    {
        [Required]
        public Guid ReportDefinitionId { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        public List<ReportFilterValueDto> FilterValues { get; set; } = new();

        public ExportFormat? ExportFormat { get; set; }

        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 50;
    }

    public class ReportFilterValueDto
    {
        [Required]
        public Guid FilterId { get; set; }

        [Required]
        public string FieldName { get; set; } = string.Empty;

        public string? Value { get; set; }
    }

}