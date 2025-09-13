using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class BankTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BankAccountId { get; set; }
        public string TransactionNumber { get; set; }
        public BankTransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string Description { get; set; }
        public string ReferenceNumber { get; set; }
        public string Memo { get; set; }
        public bool IsReconciled { get; set; } = false;
        public DateTime? ReconciledDate { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public BankAccount BankAccount { get; set; }
        public Company Company { get; set; }
    }
}