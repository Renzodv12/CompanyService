using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Enums;
using MediatR;

namespace CompanyService.Application.Commands.Finance
{
    public class CreateCashFlowCommand : IRequest<CashFlowResponseDto>
    {
        public string Description { get; set; } = string.Empty;
        public CashFlowType CashFlowType { get; set; }
        public decimal Amount { get; set; }
        public bool IsInflow { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Category { get; set; }
        public string? ReferenceNumber { get; set; }
        public Guid? RelatedAccountId { get; set; }
        public Guid? RelatedBankAccountId { get; set; }
        public string? Notes { get; set; }
        public Guid CompanyId { get; set; }
    }
}