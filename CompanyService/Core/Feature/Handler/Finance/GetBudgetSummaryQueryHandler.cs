using MediatR;
using CompanyService.Core.Feature.Querys.Finance;
using CompanyService.Core.DTOs.Finance;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para obtener el resumen de presupuestos
    /// </summary>
    public class GetBudgetSummaryQueryHandler : IRequestHandler<GetBudgetSummaryQuery, BudgetSummaryDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetBudgetSummaryQueryHandler> _logger;

        public GetBudgetSummaryQueryHandler(
            ApplicationDbContext context,
            ILogger<GetBudgetSummaryQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BudgetSummaryDto> Handle(GetBudgetSummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting budget summary for company {CompanyId}", request.CompanyId);

                var query = _context.Budgets
                    .Include(b => b.BudgetLines)
                    .Where(b => b.CompanyId == request.CompanyId && b.IsActive);

                // Aplicar filtros de fecha
                if (request.Year.HasValue)
                    query = query.Where(b => b.Year == request.Year.Value);

                if (request.Month.HasValue)
                    query = query.Where(b => b.Month == request.Month.Value);

                if (request.StartDate.HasValue)
                    query = query.Where(b => b.CreatedAt >= request.StartDate.Value);

                if (request.EndDate.HasValue)
                    query = query.Where(b => b.CreatedAt <= request.EndDate.Value);

                var budgets = await query.ToListAsync(cancellationToken);

                if (!budgets.Any())
                {
                    return new BudgetSummaryDto
                    {
                        PeriodStart = request.StartDate ?? DateTime.UtcNow.AddMonths(-12),
                        PeriodEnd = request.EndDate ?? DateTime.UtcNow
                    };
                }

                // Calcular métricas generales
                var totalBudgeted = budgets.Sum(b => b.BudgetedAmount);
                var totalActual = budgets.Sum(b => b.ActualAmount);
                var totalVariance = totalActual - totalBudgeted;
                var variancePercentage = totalBudgeted != 0 ? (totalVariance / totalBudgeted) * 100 : 0;

                // Contar presupuestos por estado
                var overBudgetCount = budgets.Count(b => b.ActualAmount > b.BudgetedAmount);
                var underBudgetCount = budgets.Count(b => b.ActualAmount < b.BudgetedAmount);
                var onTargetCount = budgets.Count(b => Math.Abs(b.ActualAmount - b.BudgetedAmount) <= (b.BudgetedAmount * 0.05m)); // 5% de tolerancia

                // Resumen por categorías
                var categorySummaries = budgets
                    .GroupBy(b => b.Category ?? "Sin categoría")
                    .Select(g => new BudgetCategorySummaryDto
                    {
                        Category = g.Key,
                        BudgetedAmount = g.Sum(b => b.BudgetedAmount),
                        ActualAmount = g.Sum(b => b.ActualAmount),
                        Variance = g.Sum(b => b.ActualAmount) - g.Sum(b => b.BudgetedAmount),
                        BudgetCount = g.Count()
                    })
                    .ToList();

                foreach (var category in categorySummaries)
                {
                    category.VariancePercentage = category.BudgetedAmount != 0 
                        ? (category.Variance / category.BudgetedAmount) * 100 
                        : 0;
                }

                // Estados de presupuestos
                var budgetStatuses = new List<BudgetStatusDto>
                {
                    new BudgetStatusDto
                    {
                        Status = "Sobre presupuesto",
                        Count = overBudgetCount,
                        TotalAmount = budgets.Where(b => b.ActualAmount > b.BudgetedAmount).Sum(b => b.ActualAmount),
                        Percentage = budgets.Count > 0 ? (overBudgetCount * 100.0m) / budgets.Count : 0
                    },
                    new BudgetStatusDto
                    {
                        Status = "Por debajo del presupuesto",
                        Count = underBudgetCount,
                        TotalAmount = budgets.Where(b => b.ActualAmount < b.BudgetedAmount).Sum(b => b.ActualAmount),
                        Percentage = budgets.Count > 0 ? (underBudgetCount * 100.0m) / budgets.Count : 0
                    },
                    new BudgetStatusDto
                    {
                        Status = "En objetivo",
                        Count = onTargetCount,
                        TotalAmount = budgets.Where(b => Math.Abs(b.ActualAmount - b.BudgetedAmount) <= (b.BudgetedAmount * 0.05m)).Sum(b => b.ActualAmount),
                        Percentage = budgets.Count > 0 ? (onTargetCount * 100.0m) / budgets.Count : 0
                    }
                };

                // Tendencias mensuales (últimos 12 meses)
                var monthlyTrends = budgets
                    .GroupBy(b => new { b.Year, b.Month })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .Take(12)
                    .Select(g => new BudgetTrendDto
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month ?? 0,
                        MonthName = GetMonthName(g.Key.Month ?? 0),
                        BudgetedAmount = g.Sum(b => b.BudgetedAmount),
                        ActualAmount = g.Sum(b => b.ActualAmount),
                        Variance = g.Sum(b => b.ActualAmount) - g.Sum(b => b.BudgetedAmount)
                    })
                    .ToList();

                foreach (var trend in monthlyTrends)
                {
                    trend.VariancePercentage = trend.BudgetedAmount != 0 
                        ? (trend.Variance / trend.BudgetedAmount) * 100 
                        : 0;
                }

                var summary = new BudgetSummaryDto
                {
                    TotalBudgeted = totalBudgeted,
                    TotalActual = totalActual,
                    TotalVariance = totalVariance,
                    VariancePercentage = variancePercentage,
                    BudgetCount = budgets.Count,
                    OverBudgetCount = overBudgetCount,
                    UnderBudgetCount = underBudgetCount,
                    OnTargetCount = onTargetCount,
                    PeriodStart = request.StartDate ?? budgets.Min(b => b.CreatedAt),
                    PeriodEnd = request.EndDate ?? budgets.Max(b => b.CreatedAt),
                    CategorySummaries = categorySummaries,
                    BudgetStatuses = budgetStatuses,
                    MonthlyTrends = monthlyTrends
                };

                _logger.LogInformation("Successfully generated budget summary for company {CompanyId}", request.CompanyId);

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budget summary for company {CompanyId}", request.CompanyId);
                throw;
            }
        }

        private static string GetMonthName(int month)
        {
            return month switch
            {
                1 => "Enero",
                2 => "Febrero",
                3 => "Marzo",
                4 => "Abril",
                5 => "Mayo",
                6 => "Junio",
                7 => "Julio",
                8 => "Agosto",
                9 => "Septiembre",
                10 => "Octubre",
                11 => "Noviembre",
                12 => "Diciembre",
                _ => $"Mes {month}"
            };
        }
    }
}
