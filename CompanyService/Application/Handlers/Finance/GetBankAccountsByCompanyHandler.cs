using CompanyService.Application.Queries.Finance;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Application.Handlers.Finance
{
    public class GetBankAccountsByCompanyHandler : IRequestHandler<GetBankAccountsByCompanyQuery, IEnumerable<BankAccountResponseDto>>
    {
        private readonly IBankReconciliationService _bankReconciliationService;

        public GetBankAccountsByCompanyHandler(IBankReconciliationService bankReconciliationService)
        {
            _bankReconciliationService = bankReconciliationService;
        }

        public async Task<IEnumerable<BankAccountResponseDto>> Handle(GetBankAccountsByCompanyQuery request, CancellationToken cancellationToken)
        {
            return await _bankReconciliationService.GetBankAccountsByCompanyAsync(request.CompanyId);
        }
    }
}