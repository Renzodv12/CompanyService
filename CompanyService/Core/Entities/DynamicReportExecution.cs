using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.DTOs.DynamicReports;

namespace CompanyService.Core.Entities
{
    public class DynamicReportExecution
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public Guid ReportDefinitionId { get; set; }
        
        [Required]
        public Guid ExecutedByUserId { get; set; }
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        public ReportExecutionStatus Status { get; set; } = ReportExecutionStatus.Pending;
        
        public string? FiltersApplied { get; set; } // JSON con los filtros aplicados
        
        public int RowCount { get; set; } = 0;
        
        public TimeSpan ExecutionDuration { get; set; }
        
        public string? ErrorMessage { get; set; }
        
        public string? ExportFormat { get; set; } // csv, xlsx, pdf
        
        public string? ExportFileName { get; set; }
        
        public long? FileSizeBytes { get; set; }
        
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ExpiresAt { get; set; }
        
        // Navegación
        public ReportDefinition ReportDefinition { get; set; }
        public Company Company { get; set; }
    }
}