using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class AccountsReceivable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string InvoiceNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? SaleId { get; set; } // Referencia a la venta si aplica
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; } = 0;
        public decimal RemainingAmount => TotalAmount - PaidAmount;
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public AccountReceivableStatus Status { get; set; } = AccountReceivableStatus.Pending;
        public string Description { get; set; }
        public string Notes { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navegación
        public Customer Customer { get; set; }
        public Sale Sale { get; set; }
        public Company Company { get; set; }
        public ICollection<AccountsReceivablePayment> Payments { get; set; } = new List<AccountsReceivablePayment>();
    }
}