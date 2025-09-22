using CompanyService.Application.Queries.Finance;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Application.Handlers.Finance
{
    /// <summary>
    /// Handler for GetCashFlowReportQuery
    /// </summary>
    public class GetCashFlowReportQueryHandler : IRequestHandler<GetCashFlowReportQuery, CashFlowReportDto>
    {
        private readonly ICashFlowService _cashFlowService;

        public GetCashFlowReportQueryHandler(ICashFlowService cashFlowService)
        {
            _cashFlowService = cashFlowService;
        }

        public async Task<CashFlowReportDto> Handle(GetCashFlowReportQuery request, CancellationToken cancellationToken)
        {
            // Get cash flows for the specified company and date range
            var cashFlows = await _cashFlowService.GetCashFlowsByCompanyAsync(
                request.CompanyId, 
                request.StartDate, 
                request.EndDate);

            var totalInflow = cashFlows.Where(cf => cf.IsInflow).Sum(cf => cf.Amount);
            var totalOutflow = cashFlows.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount);
            var filteredCashFlows = cashFlows.Where(cf => cf.TransactionDate >= request.StartDate && cf.TransactionDate <= request.EndDate);

            // Return a comprehensive cash flow report
            return new CashFlowReportDto
            {
                CompanyId = request.CompanyId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalInflow = totalInflow,
                TotalOutflow = totalOutflow,
                NetCashFlow = totalInflow - totalOutflow,
                OperatingCashFlow = filteredCashFlows.Where(cf => cf.Category != null && cf.Category.Contains("Operating")).Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                InvestingCashFlow = filteredCashFlows.Where(cf => cf.Category != null && cf.Category.Contains("Investing")).Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                FinancingCashFlow = filteredCashFlows.Where(cf => cf.Category != null && cf.Category.Contains("Financing")).Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                CashFlows = filteredCashFlows.ToList(),
                TransactionCount = filteredCashFlows.Count(),
                OperatingActivities = new List<CashFlowActivityDto>(),
                InvestingActivities = new List<CashFlowActivityDto>(),
                FinancingActivities = new List<CashFlowActivityDto>()
            };
        }
    }
}