using CompanyService.Core.DTOs.DynamicReports;
using MediatR;

namespace CompanyService.Application.Commands.DynamicReports
{
    public class UpdateReportDefinitionCommand : IRequest<ReportDefinitionResponseDto>
    {
        public Guid Id { get; set; }
        public UpdateReportDefinitionDto ReportDefinition { get; set; } = null!;
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
    }
}