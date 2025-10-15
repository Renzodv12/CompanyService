using MediatR;
using CompanyService.Core.DTOs.Finance;

namespace CompanyService.Core.Feature.Commands.Finance
{
    /// <summary>
    /// Comando para comparar múltiples presupuestos
    /// </summary>
    public class CompareBudgetsCommand : IRequest<BudgetComparisonDto>
    {
        public List<Guid> BudgetIds { get; set; } = new();
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class BudgetComparisonDto
    {
        public List<BudgetComparisonItemDto> Budgets { get; set; } = new();
        public BudgetComparisonSummaryDto Summary { get; set; } = new();
        public List<BudgetComparisonCategoryDto> CategoryComparison { get; set; } = new();
    }

    public class BudgetComparisonItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public decimal BudgetedAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class BudgetComparisonSummaryDto
    {
        public decimal TotalBudgeted { get; set; }
        public decimal TotalActual { get; set; }
        public decimal TotalVariance { get; set; }
        public decimal AverageVariancePercentage { get; set; }
        public int BudgetCount { get; set; }
        public int OverBudgetCount { get; set; }
        public int UnderBudgetCount { get; set; }
    }

    public class BudgetComparisonCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public List<decimal> BudgetedAmounts { get; set; } = new();
        public List<decimal> ActualAmounts { get; set; } = new();
        public List<decimal> Variances { get; set; } = new();
        public decimal AverageVariancePercentage { get; set; }
    }
}
