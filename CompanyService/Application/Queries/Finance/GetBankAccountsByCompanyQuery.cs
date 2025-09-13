using CompanyService.Core.DTOs.Finance;
using MediatR;

namespace CompanyService.Application.Queries.Finance
{
    public class GetBankAccountsByCompanyQuery : IRequest<IEnumerable<BankAccountResponseDto>>
    {
        public Guid CompanyId { get; set; }
    }
}