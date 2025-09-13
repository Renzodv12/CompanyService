using CompanyService.Application.Queries.Finance;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Application.Handlers.Finance
{
    public class GetCashFlowsByCompanyHandler : IRequestHandler<GetCashFlowsByCompanyQuery, IEnumerable<CashFlowResponseDto>>
    {
        private readonly ICashFlowService _cashFlowService;

        public GetCashFlowsByCompanyHandler(ICashFlowService cashFlowService)
        {
            _cashFlowService = cashFlowService;
        }

        public async Task<IEnumerable<CashFlowResponseDto>> Handle(GetCashFlowsByCompanyQuery request, CancellationToken cancellationToken)
        {
            return await _cashFlowService.GetCashFlowsByCompanyAsync(request.CompanyId, request.StartDate, request.EndDate);
        }
    }
}