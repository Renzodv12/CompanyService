using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Enums;
using MediatR;

namespace CompanyService.Application.Commands.Finance
{
    public class CreateBankTransactionCommand : IRequest<BankTransactionResponseDto>
    {
        public Guid BankAccountId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public BankTransactionType Type { get; set; }
        public string? Category { get; set; }
        public string? ReferenceNumber { get; set; }
        public Guid CompanyId { get; set; }
    }
}