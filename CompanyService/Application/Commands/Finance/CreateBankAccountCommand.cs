using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Enums;
using MediatR;

namespace CompanyService.Application.Commands.Finance
{
    public class CreateBankAccountCommand : IRequest<BankAccountResponseDto>
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public BankAccountType AccountType { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal InitialBalance { get; set; }
        public Guid CompanyId { get; set; }
    }
}