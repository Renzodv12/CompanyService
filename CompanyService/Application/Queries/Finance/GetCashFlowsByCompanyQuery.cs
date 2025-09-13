using CompanyService.Core.DTOs.Finance;
using MediatR;

namespace CompanyService.Application.Queries.Finance
{
    public class GetCashFlowsByCompanyQuery : IRequest<IEnumerable<CashFlowResponseDto>>
    {
        public Guid CompanyId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}