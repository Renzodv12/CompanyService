using CompanyService.Application.Commands.Finance;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Application.Handlers.Finance
{
    public class CreateBankAccountHandler : IRequestHandler<CreateBankAccountCommand, BankAccountResponseDto>
    {
        private readonly IBankReconciliationService _bankReconciliationService;

        public CreateBankAccountHandler(IBankReconciliationService bankReconciliationService)
        {
            _bankReconciliationService = bankReconciliationService;
        }

        public async Task<BankAccountResponseDto> Handle(CreateBankAccountCommand request, CancellationToken cancellationToken)
        {
            var dto = new CreateBankAccountDto
            {
                AccountNumber = request.AccountNumber,
                BankName = request.BankName,
                AccountName = request.AccountName,
                AccountType = request.AccountType.ToString(),
                Currency = request.Currency,
                InitialBalance = request.InitialBalance,
                CompanyId = request.CompanyId
            };

            return await _bankReconciliationService.CreateBankAccountAsync(dto);
        }
    }
}