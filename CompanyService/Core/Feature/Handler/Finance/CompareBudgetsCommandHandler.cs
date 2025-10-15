using MediatR;
using CompanyService.Core.Feature.Commands.Finance;
using CompanyService.Core.DTOs.Finance;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para comparar múltiples presupuestos
    /// </summary>
    public class CompareBudgetsCommandHandler : IRequestHandler<CompareBudgetsCommand, BudgetComparisonDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompareBudgetsCommandHandler> _logger;

        public CompareBudgetsCommandHandler(
            ApplicationDbContext context,
            ILogger<CompareBudgetsCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BudgetComparisonDto> Handle(CompareBudgetsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Comparing {Count} budgets for company {CompanyId}",
                    request.BudgetIds.Count, request.CompanyId);

                if (request.BudgetIds.Count < 2)
                {
                    throw new ArgumentException("Se requieren al menos 2 presupuestos para comparar");
                }

                if (request.BudgetIds.Count > 10)
                {
                    throw new ArgumentException("No se pueden comparar más de 10 presupuestos a la vez");
                }

                // Obtener los presupuestos
                var budgets = await _context.Budgets
                    .Include(b => b.BudgetLines)
                    .Where(b => request.BudgetIds.Contains(b.Id) && b.CompanyId == request.CompanyId && b.IsActive)
                    .ToListAsync(cancellationToken);

                if (budgets.Count != request.BudgetIds.Count)
                {
                    throw new ArgumentException("Algunos presupuestos no fueron encontrados");
                }

                // Crear los elementos de comparación
                var budgetItems = budgets.Select(b => new BudgetComparisonItemDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Period = GetPeriodDescription(b.Year, b.Month),
                    BudgetedAmount = b.BudgetedAmount,
                    ActualAmount = b.ActualAmount,
                    Variance = b.Variance,
                    VariancePercentage = b.VariancePercentage,
                    Status = GetBudgetStatus(b.VariancePercentage),
                    CreatedAt = b.CreatedAt
                }).ToList();

                // Calcular resumen
                var summary = new BudgetComparisonSummaryDto
                {
                    TotalBudgeted = budgets.Sum(b => b.BudgetedAmount),
                    TotalActual = budgets.Sum(b => b.ActualAmount),
                    TotalVariance = budgets.Sum(b => b.Variance),
                    BudgetCount = budgets.Count,
                    OverBudgetCount = budgets.Count(b => b.Variance > 0),
                    UnderBudgetCount = budgets.Count(b => b.Variance < 0)
                };

                summary.AverageVariancePercentage = summary.BudgetCount > 0 
                    ? budgets.Average(b => b.VariancePercentage) 
                    : 0;

                // Comparación por categorías
                var categoryComparison = budgets
                    .SelectMany(b => b.BudgetLines)
                    .GroupBy(bl => bl.LineItem ?? "Sin categoría")
                    .Select(g => new BudgetComparisonCategoryDto
                    {
                        Category = g.Key,
                        BudgetedAmounts = budgets.Select(b => 
                            b.BudgetLines.Where(bl => bl.LineItem == g.Key).Sum(bl => bl.BudgetedAmount)).ToList(),
                        ActualAmounts = budgets.Select(b => 
                            b.BudgetLines.Where(bl => bl.LineItem == g.Key).Sum(bl => bl.ActualAmount)).ToList(),
                        Variances = budgets.Select(b => 
                            b.BudgetLines.Where(bl => bl.LineItem == g.Key).Sum(bl => bl.Variance)).ToList()
                    })
                    .ToList();

                foreach (var category in categoryComparison)
                {
                    var totalBudgeted = category.BudgetedAmounts.Sum();
                    var totalActual = category.ActualAmounts.Sum();
                    category.AverageVariancePercentage = totalBudgeted != 0 
                        ? ((totalActual - totalBudgeted) / totalBudgeted) * 100 
                        : 0;
                }

                var comparison = new BudgetComparisonDto
                {
                    Budgets = budgetItems,
                    Summary = summary,
                    CategoryComparison = categoryComparison
                };

                _logger.LogInformation("Successfully compared {Count} budgets for company {CompanyId}",
                    request.BudgetIds.Count, request.CompanyId);

                return comparison;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing budgets for company {CompanyId}", request.CompanyId);
                throw;
            }
        }

        private static string GetPeriodDescription(int year, int? month)
        {
            if (!month.HasValue)
                return $"Año {year}";

            return month.Value switch
            {
                1 => $"Enero {year}",
                2 => $"Febrero {year}",
                3 => $"Marzo {year}",
                4 => $"Abril {year}",
                5 => $"Mayo {year}",
                6 => $"Junio {year}",
                7 => $"Julio {year}",
                8 => $"Agosto {year}",
                9 => $"Septiembre {year}",
                10 => $"Octubre {year}",
                11 => $"Noviembre {year}",
                12 => $"Diciembre {year}",
                _ => $"{month:D2}/{year}"
            };
        }

        private static string GetBudgetStatus(decimal variancePercentage)
        {
            return variancePercentage switch
            {
                > 5 => "Sobre presupuesto",
                < -5 => "Por debajo del presupuesto",
                _ => "En objetivo"
            };
        }
    }
}
