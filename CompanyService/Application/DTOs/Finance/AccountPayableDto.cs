using CompanyService.Core.Enums;

namespace CompanyService.Application.DTOs.Finance;

public class AccountPayableDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime IssueDate { get; set; }
    public AccountPayableStatus Status { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public Guid CompanyId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}