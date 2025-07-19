namespace CompanyService.Core.Entities
{
    public class JournalEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EntryNumber { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.UtcNow;
        public Guid AccountId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; } // Referencia a venta, compra, etc.
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Account Account { get; set; }
        public Company Company { get; set; }
    }
}
