using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.Finance
{
    public class CashFlowResponseDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public CashFlowType Type { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsInflow { get; set; }
        public string FlowDirection { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string? Category { get; set; }
        public string? ReferenceNumber { get; set; }
        public Guid? RelatedAccountId { get; set; }
        public string? RelatedAccountName { get; set; }
        public Guid? RelatedBankAccountId { get; set; }
        public string? RelatedBankAccountName { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CashFlowSummaryDto
    {
        public decimal TotalInflow { get; set; }
        public decimal TotalOutflow { get; set; }
        public decimal NetCashFlow { get; set; }
        public decimal OperatingCashFlow { get; set; }
        public decimal InvestingCashFlow { get; set; }
        public decimal FinancingCashFlow { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}