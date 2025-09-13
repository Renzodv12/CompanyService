using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class AccountsPayable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string InvoiceNumber { get; set; }
        public Guid SupplierId { get; set; }
        public Guid? PurchaseId { get; set; } // Referencia a la compra si aplica
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; } = 0;
        public decimal RemainingAmount => TotalAmount - PaidAmount;
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public AccountPayableStatus Status { get; set; } = AccountPayableStatus.Pending;
        public string Description { get; set; }
        public string Notes { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navegación
        public Supplier Supplier { get; set; }
        public Purchase Purchase { get; set; }
        public Company Company { get; set; }
        public ICollection<AccountsPayablePayment> Payments { get; set; } = new List<AccountsPayablePayment>();
    }
}