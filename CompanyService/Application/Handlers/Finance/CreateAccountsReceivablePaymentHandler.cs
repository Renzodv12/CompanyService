using CompanyService.Application.Commands.Finance;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Application.Handlers.Finance
{
    public class CreateAccountsReceivablePaymentHandler : IRequestHandler<CreateAccountsReceivablePaymentCommand, AccountsReceivablePaymentResponseDto>
    {
        private readonly IFinanceService _financeService;

        public CreateAccountsReceivablePaymentHandler(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        public async Task<AccountsReceivablePaymentResponseDto> Handle(CreateAccountsReceivablePaymentCommand request, CancellationToken cancellationToken)
        {
            var dto = new CreateAccountsReceivablePaymentDto
            {
                AccountsReceivableId = request.AccountsReceivableId,
                Amount = request.Amount,
                PaymentDate = request.PaymentDate,
                PaymentMethod = request.PaymentMethod,
                ReferenceNumber = request.ReferenceNumber,
                Notes = request.Notes,
                CompanyId = request.CompanyId
            };

            return await _financeService.CreateAccountsReceivablePaymentAsync(dto);
        }
    }
}