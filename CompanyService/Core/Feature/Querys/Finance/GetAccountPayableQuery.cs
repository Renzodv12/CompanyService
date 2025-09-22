using MediatR;
using CompanyService.Core.DTOs.Finance;

namespace CompanyService.Core.Feature.Querys.Finance
{
    /// <summary>
    /// Query para obtener una cuenta por pagar específica
    /// </summary>
    public class GetAccountPayableQuery : IRequest<AccountsPayableResponseDto>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}