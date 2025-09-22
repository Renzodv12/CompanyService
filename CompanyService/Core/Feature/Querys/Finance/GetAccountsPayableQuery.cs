using MediatR;
using CompanyService.Core.DTOs.Finance;

namespace CompanyService.Core.Feature.Querys.Finance
{
    /// <summary>
    /// Query para obtener la lista de cuentas por pagar
    /// </summary>
    public class GetAccountsPayableQuery : IRequest<List<AccountsPayableResponseDto>>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}