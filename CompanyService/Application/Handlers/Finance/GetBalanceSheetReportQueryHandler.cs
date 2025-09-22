using CompanyService.Application.Queries.Finance;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Application.Handlers.Finance
{
    /// <summary>
    /// Handler for GetBalanceSheetReportQuery
    /// </summary>
    public class GetBalanceSheetReportQueryHandler : IRequestHandler<GetBalanceSheetReportQuery, BalanceSheetReportDto>
    {
        private readonly ICashFlowService _cashFlowService;

        public GetBalanceSheetReportQueryHandler(ICashFlowService cashFlowService)
        {
            _cashFlowService = cashFlowService;
        }

        public async Task<BalanceSheetReportDto> Handle(GetBalanceSheetReportQuery request, CancellationToken cancellationToken)
        {
            // Get cash flows up to the specified date to calculate balance sheet
            var cashFlows = await _cashFlowService.GetCashFlowsByCompanyAsync(request.CompanyId);
            
            var filteredCashFlows = cashFlows.Where(cf => cf.TransactionDate <= request.AsOfDate);

            var totalAssets = filteredCashFlows
                .Where(cf => cf.IsInflow)
                .Sum(cf => cf.Amount);

            var totalLiabilities = filteredCashFlows
                .Where(cf => !cf.IsInflow && cf.Category != null && cf.Category.Contains("Liability"))
                .Sum(cf => cf.Amount);

            var totalEquity = totalAssets - totalLiabilities;

            return new BalanceSheetReportDto
            {
                CompanyId = request.CompanyId,
                AsOfDate = request.AsOfDate,
                TotalAssets = totalAssets,
                TotalLiabilities = totalLiabilities,
                TotalEquity = totalEquity,
                Assets = filteredCashFlows
                    .Where(cf => cf.IsInflow)
                    .GroupBy(cf => cf.Category ?? "Other")
                    .Select(g => new AssetLineItem
                    {
                        Category = g.Key,
                        Amount = g.Sum(cf => cf.Amount),
                        IsCurrentAsset = true // Simplified logic
                    }).ToList(),
                Liabilities = filteredCashFlows
                    .Where(cf => !cf.IsInflow && cf.Category != null && cf.Category.Contains("Liability"))
                    .GroupBy(cf => cf.Category)
                    .Select(g => new LiabilityLineItem
                    {
                        Category = g.Key,
                        Amount = g.Sum(cf => cf.Amount),
                        IsCurrentLiability = true // Simplified logic
                    }).ToList(),
                Equity = new List<EquityLineItem>
                {
                    new EquityLineItem
                    {
                        Category = "Retained Earnings",
                        Amount = totalEquity
                    }
                }
            };
        }
    }
}