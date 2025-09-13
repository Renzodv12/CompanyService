namespace CompanyService.Core.Entities
{
    public class BankAccount
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; } // Código del banco
        public string AccountType { get; set; } // Corriente, Ahorros, etc.
        public decimal Balance { get; set; } = 0;
        public string Currency { get; set; } = "USD";
        public bool IsActive { get; set; } = true;
        public string Description { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navegación
        public Company Company { get; set; }
        public ICollection<BankTransaction> BankTransactions { get; set; } = new List<BankTransaction>();
    }
}