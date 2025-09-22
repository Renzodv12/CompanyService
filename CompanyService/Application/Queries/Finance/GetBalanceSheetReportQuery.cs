using CompanyService.Core.DTOs.Finance;
using MediatR;

namespace CompanyService.Application.Queries.Finance
{
    /// <summary>
    /// Query to get balance sheet report for a company as of a specific date
    /// </summary>
    public class GetBalanceSheetReportQuery : IRequest<BalanceSheetReportDto>
    {
        public Guid CompanyId { get; set; }
        public DateTime AsOfDate { get; set; }
        public Guid UserId { get; set; }
    }
}