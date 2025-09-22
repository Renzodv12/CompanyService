using CompanyService.Core.DTOs.Finance;
using MediatR;

namespace CompanyService.Application.Queries.Finance
{
    /// <summary>
    /// Query to get profit and loss report for a company within a date range
    /// </summary>
    public class GetProfitLossReportQuery : IRequest<ProfitLossReportDto>
    {
        public Guid CompanyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid UserId { get; set; }
    }
}