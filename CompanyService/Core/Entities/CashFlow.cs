using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class CashFlow
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Description { get; set; }
        public CashFlowType Type { get; set; }
        public decimal Amount { get; set; }
        public bool IsInflow { get; set; } // true = entrada, false = salida
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string Category { get; set; }
        public string ReferenceNumber { get; set; }
        public Guid? AccountId { get; set; } // Cuenta contable relacionada
        public Guid? RelatedAccountId { get; set; } // ID de cuenta contable relacionada
        public Guid? BankAccountId { get; set; } // Cuenta bancaria relacionada
        public Guid? RelatedBankAccountId { get; set; } // ID de cuenta bancaria relacionada
        public string Notes { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navegación
        public Account Account { get; set; }
        public Account RelatedAccount { get; set; }
        public BankAccount BankAccount { get; set; }
        public BankAccount RelatedBankAccount { get; set; }
        public Company Company { get; set; }
    }
}