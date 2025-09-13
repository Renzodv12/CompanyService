using CompanyService.Application.Commands.Finance;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Application.Handlers.Finance
{
    public class CreateBankTransactionHandler : IRequestHandler<CreateBankTransactionCommand, BankTransactionResponseDto>
    {
        private readonly IBankReconciliationService _bankReconciliationService;

        public CreateBankTransactionHandler(IBankReconciliationService bankReconciliationService)
        {
            _bankReconciliationService = bankReconciliationService;
        }

        public async Task<BankTransactionResponseDto> Handle(CreateBankTransactionCommand request, CancellationToken cancellationToken)
        {
            var dto = new CreateBankTransactionDto
            {
                BankAccountId = request.BankAccountId,
                TransactionDate = request.TransactionDate,
                Description = request.Description,
                Amount = request.Amount,
                Type = request.Type,
                ReferenceNumber = request.ReferenceNumber,
                CompanyId = request.CompanyId
            };

            return await _bankReconciliationService.CreateBankTransactionAsync(dto);
        }
    }
}