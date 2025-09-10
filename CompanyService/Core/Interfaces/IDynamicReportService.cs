using CompanyService.Core.DTOs.DynamicReports;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Interfaces
{
    public interface IDynamicReportService
    {
        // Report Definition Management
        Task<ReportDefinitionResponseDto> CreateReportDefinitionAsync(CreateReportDefinitionDto dto, Guid userId);
        Task<ReportDefinitionResponseDto> UpdateReportDefinitionAsync(Guid id, UpdateReportDefinitionDto dto, Guid userId, Guid companyId);
        Task<bool> DeleteReportDefinitionAsync(Guid id, Guid userId, Guid companyId);
        Task<ReportDefinitionResponseDto?> GetReportDefinitionAsync(Guid id, Guid companyId);
        Task<IEnumerable<ReportDefinitionListDto>> GetReportDefinitionsAsync(Guid companyId, bool includeShared = true);
        
        // Report Execution
        Task<ReportExecutionResponseDto> ExecuteReportAsync(ExecuteReportDto dto, Guid userId);
        Task<ReportExecutionResponseDto?> GetReportExecutionAsync(Guid executionId, Guid companyId);
        Task<IEnumerable<ReportExecutionListDto>> GetReportExecutionsAsync(Guid companyId, Guid? userId = null, int pageNumber = 1, int pageSize = 50);
        
        // Data Export
        Task<byte[]> ExportReportAsync(Guid executionId, ExportFormat format, Guid companyId);
        
        // Validation and Security
        Task<bool> ValidateReportDefinitionAsync(CreateReportDefinitionDto dto, Guid companyId);
        Task<bool> CanUserAccessReportAsync(Guid reportDefinitionId, Guid userId, Guid companyId);
        Task<bool> CanUserExecuteReportAsync(Guid reportDefinitionId, Guid userId, Guid companyId);
        
        // Entity Schema Discovery
        Task<IEnumerable<string>> GetAvailableEntitiesAsync(Guid companyId);
        Task<IEnumerable<EntityFieldDto>> GetEntityFieldsAsync(string entityName, Guid companyId);
    }

    public class EntityFieldDto
    {
        public string FieldName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public bool IsFilterable { get; set; }
        public bool IsAggregatable { get; set; }
        public string? Description { get; set; }
    }
}