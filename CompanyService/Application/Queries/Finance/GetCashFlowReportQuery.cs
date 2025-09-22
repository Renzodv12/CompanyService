using CompanyService.Core.DTOs.Finance;
using MediatR;

namespace CompanyService.Application.Queries.Finance
{
    /// <summary>
    /// Query to get cash flow report for a company within a date range
    /// </summary>
    public class GetCashFlowReportQuery : IRequest<CashFlowReportDto>
    {
        public Guid CompanyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid UserId { get; set; }
    }
}