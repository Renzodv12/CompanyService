namespace CompanyService.Core.DTOs.Finance
{
    public class BudgetResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Year { get; set; }
        public int? Month { get; set; }
        public string Period { get; set; } = string.Empty;
        public decimal BudgetedAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
        public Guid? AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<BudgetLineResponseDto> BudgetLines { get; set; } = new();
    }

    public class BudgetLineResponseDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal BudgetedAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
        public Guid? AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? Category { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class BudgetSummaryDto
    {
        public decimal TotalBudgeted { get; set; }
        public decimal TotalActual { get; set; }
        public decimal TotalVariance { get; set; }
        public decimal VariancePercentage { get; set; }
        public int BudgetCount { get; set; }
        public int OverBudgetCount { get; set; }
        public int UnderBudgetCount { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}