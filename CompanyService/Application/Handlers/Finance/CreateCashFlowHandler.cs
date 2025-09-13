using CompanyService.Application.Commands.Finance;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Application.Handlers.Finance
{
    public class CreateCashFlowHandler : IRequestHandler<CreateCashFlowCommand, CashFlowResponseDto>
    {
        private readonly ICashFlowService _cashFlowService;

        public CreateCashFlowHandler(ICashFlowService cashFlowService)
        {
            _cashFlowService = cashFlowService;
        }

        public async Task<CashFlowResponseDto> Handle(CreateCashFlowCommand request, CancellationToken cancellationToken)
        {
            var dto = new CreateCashFlowDto
            {
                Description = request.Description,
                Type = request.CashFlowType,
                Amount = request.Amount,
                IsInflow = request.IsInflow,
                TransactionDate = request.TransactionDate,
                Category = request.Category,
                ReferenceNumber = request.ReferenceNumber,
                RelatedAccountId = request.RelatedAccountId,
                RelatedBankAccountId = request.RelatedBankAccountId,
                Notes = request.Notes,
                CompanyId = request.CompanyId
            };

            return await _cashFlowService.CreateCashFlowAsync(dto);
        }
    }
}