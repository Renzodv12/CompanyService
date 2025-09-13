using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.Finance
{
    public class BankTransactionResponseDto
    {
        public Guid Id { get; set; }
        public Guid BankAccountId { get; set; }
        public string BankAccountName { get; set; } = string.Empty;
        public string TransactionNumber { get; set; } = string.Empty;
        public BankTransactionType Type { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ReferenceNumber { get; set; }
        public string? Payee { get; set; }
        public string? Notes { get; set; }
        public Guid? RelatedAccountId { get; set; }
        public string? RelatedAccountName { get; set; }
        public bool IsReconciled { get; set; }
        public DateTime? ReconciledDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}