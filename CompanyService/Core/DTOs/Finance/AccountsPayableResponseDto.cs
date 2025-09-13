using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.Finance
{
    public class AccountsPayableResponseDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public Guid? PurchaseId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public AccountPayableStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public bool IsOverdue { get; set; }
        public int DaysOverdue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<AccountsPayablePaymentResponseDto> Payments { get; set; } = new();
    }

    public class AccountsPayablePaymentResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentMethodName { get; set; } = string.Empty;
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}