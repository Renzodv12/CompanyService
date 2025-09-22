using MediatR;
using CompanyService.Core.DTOs.Finance;

namespace CompanyService.Core.Feature.Querys.Finance
{
    /// <summary>
    /// Query para obtener la lista de cuentas por cobrar
    /// </summary>
    public class GetAccountsReceivableQuery : IRequest<List<AccountsReceivableResponseDto>>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}