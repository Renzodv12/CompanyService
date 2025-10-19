using MediatR;
using CompanyService.Core.DTOs.Finance;

namespace CompanyService.Core.Feature.Querys.Finance
{
    /// <summary>
    /// Query para obtener todas las cuentas contables
    /// </summary>
    public class GetChartOfAccountsQuery : IRequest<List<ChartOfAccountsDto>>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public bool? IsActive { get; set; }
        public string? Type { get; set; }
    }

    /// <summary>
    /// Query para obtener una cuenta contable específica
    /// </summary>
    public class GetChartOfAccountsByIdQuery : IRequest<ChartOfAccountsDto>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}

