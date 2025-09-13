using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class AccountsReceivablePayment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AccountsReceivableId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; } // Número de cheque, transferencia, etc.
        public string Notes { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public AccountsReceivable AccountsReceivable { get; set; }
        public Company Company { get; set; }
    }
}