using CompanyService.Core.DTOs.DynamicReports;
using MediatR;

namespace CompanyService.Application.Queries.DynamicReports
{
    public class GetReportDefinitionsQuery : IRequest<IEnumerable<ReportDefinitionListDto>>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public bool IncludeShared { get; set; } = true;
    }
}