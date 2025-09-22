using MediatR;
using CompanyService.Core.DTOs.DynamicReports;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Feature.Commands.DynamicReports
{
    public class ExportReportExecutionCommand : IRequest<ExportResultDto>
    {
        public Guid ExecutionId { get; set; }
        public ExportFormat Format { get; set; }
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
    }
}