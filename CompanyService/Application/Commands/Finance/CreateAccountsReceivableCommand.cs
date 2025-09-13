using CompanyService.Core.DTOs.Finance;
using MediatR;

namespace CompanyService.Application.Commands.Finance
{
    public class CreateAccountsReceivableCommand : IRequest<AccountsReceivableResponseDto>
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Guid? SaleId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public Guid CompanyId { get; set; }
    }
}