using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class Account
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } // Código contable
        public string Name { get; set; }
        public string Description { get; set; }
        public AccountType Type { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? ParentAccountId { get; set; } // Para cuentas padre-hijo
        public Guid CompanyId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Account ParentAccount { get; set; }
        public Company Company { get; set; }
        public ICollection<Account> SubAccounts { get; set; } = new List<Account>();
        public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
    }
}
