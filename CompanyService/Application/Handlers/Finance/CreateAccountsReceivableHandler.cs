using CompanyService.Application.Commands.Finance;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Application.Handlers.Finance
{
    public class CreateAccountsReceivableHandler : IRequestHandler<CreateAccountsReceivableCommand, AccountsReceivableResponseDto>
    {
        private readonly IFinanceService _financeService;

        public CreateAccountsReceivableHandler(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        public async Task<AccountsReceivableResponseDto> Handle(CreateAccountsReceivableCommand request, CancellationToken cancellationToken)
        {
            var dto = new CreateAccountsReceivableDto
            {
                InvoiceNumber = request.InvoiceNumber,
                CustomerId = request.CustomerId,
                SaleId = request.SaleId,
                TotalAmount = request.TotalAmount,
                DueDate = request.DueDate,
                Description = request.Description,
                Notes = request.Notes,
                CompanyId = request.CompanyId
            };

            return await _financeService.CreateAccountsReceivableAsync(dto);
        }
    }
}