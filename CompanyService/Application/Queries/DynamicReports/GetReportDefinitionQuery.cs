using CompanyService.Core.DTOs.DynamicReports;
using MediatR;

namespace CompanyService.Application.Queries.DynamicReports
{
    public class GetReportDefinitionQuery : IRequest<ReportDefinitionResponseDto?>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}