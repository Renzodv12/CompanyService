using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Enums;
using MediatR;

namespace CompanyService.Application.Commands.Finance
{
    public class CreateAccountsReceivablePaymentCommand : IRequest<AccountsReceivablePaymentResponseDto>
    {
        public Guid AccountsReceivableId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public Guid CompanyId { get; set; }
    }
}