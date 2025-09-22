using CompanyService.Application.Queries.Finance;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Application.Handlers.Finance
{
    /// <summary>
    /// Handler for GetProfitLossReportQuery
    /// </summary>
    public class GetProfitLossReportQueryHandler : IRequestHandler<GetProfitLossReportQuery, ProfitLossReportDto>
    {
        private readonly ICashFlowService _cashFlowService;

        public GetProfitLossReportQueryHandler(ICashFlowService cashFlowService)
        {
            _cashFlowService = cashFlowService;
        }

        public async Task<ProfitLossReportDto> Handle(GetProfitLossReportQuery request, CancellationToken cancellationToken)
        {
            // Get cash flows for the period to calculate profit/loss
            var cashFlows = await _cashFlowService.GetCashFlowsByCompanyAsync(request.CompanyId);
            
            var filteredCashFlows = cashFlows.Where(cf => 
                cf.TransactionDate >= request.StartDate && cf.TransactionDate <= request.EndDate);

            var totalRevenue = filteredCashFlows
                .Where(cf => cf.IsInflow)
                .Sum(cf => cf.Amount);

            var totalExpenses = filteredCashFlows
                .Where(cf => !cf.IsInflow)
                .Sum(cf => cf.Amount);

            return new ProfitLossReportDto
            {
                CompanyId = request.CompanyId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                NetProfit = totalRevenue - totalExpenses,
                RevenueItems = filteredCashFlows
                    .Where(cf => cf.IsInflow)
                    .GroupBy(cf => cf.Category ?? "Other")
                    .Select(g => new RevenueLineItem
                    {
                        Category = g.Key,
                        Amount = g.Sum(cf => cf.Amount)
                    }).ToList(),
                ExpenseItems = filteredCashFlows
                    .Where(cf => !cf.IsInflow)
                    .GroupBy(cf => cf.Category ?? "Other")
                    .Select(g => new ExpenseLineItem
                    {
                        Category = g.Key,
                        Amount = g.Sum(cf => cf.Amount)
                    }).ToList()
            };
        }
    }
}