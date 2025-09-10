using CompanyService.Core.DTOs.DynamicReports;
using MediatR;

namespace CompanyService.Application.Queries.DynamicReports
{
    public class GetReportExecutionsQuery : IRequest<IEnumerable<ReportExecutionListDto>>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public Guid? FilterByUserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}