using CompanyService.Core.DTOs.Finance;
using MediatR;

namespace CompanyService.Application.Queries.Finance
{
    public class GetAccountsReceivableByCompanyQuery : IRequest<IEnumerable<AccountsReceivableResponseDto>>
    {
        public Guid CompanyId { get; set; }
        public bool OnlyOverdue { get; set; } = false;
    }
}