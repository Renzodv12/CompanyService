using CompanyService.Core.DTOs.DynamicReports;
using MediatR;

namespace CompanyService.Application.Commands.DynamicReports
{
    public class CreateReportDefinitionCommand : IRequest<ReportDefinitionResponseDto>
    {
        public CreateReportDefinitionDto ReportDefinition { get; set; } = null!;
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
    }
}