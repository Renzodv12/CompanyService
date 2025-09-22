namespace CompanyService.Core.DTOs.Finance
{
    public class BudgetDetailDto
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
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public List<BudgetLineDetailDto> BudgetLines { get; set; } = new();
        public BudgetSummaryDto Summary { get; set; } = new();
    }

    public class BudgetLineDetailDto
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
        public List<BudgetLineTransactionDto> Transactions { get; set; } = new();
    }

    public class BudgetLineTransactionDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Reference { get; set; }
        public string TransactionType { get; set; } = string.Empty;
    }
}