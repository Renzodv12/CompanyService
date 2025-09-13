using CompanyService.Application.Queries.Finance;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Application.Handlers.Finance
{
    public class GetAccountsReceivableByCompanyHandler : IRequestHandler<GetAccountsReceivableByCompanyQuery, IEnumerable<AccountsReceivableResponseDto>>
    {
        private readonly IFinanceService _financeService;

        public GetAccountsReceivableByCompanyHandler(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        public async Task<IEnumerable<AccountsReceivableResponseDto>> Handle(GetAccountsReceivableByCompanyQuery request, CancellationToken cancellationToken)
        {
            if (request.OnlyOverdue)
            {
                return await _financeService.GetOverdueAccountsReceivableAsync(request.CompanyId);
            }

            return await _financeService.GetAccountsReceivableByCompanyAsync(request.CompanyId);
        }
    }
}