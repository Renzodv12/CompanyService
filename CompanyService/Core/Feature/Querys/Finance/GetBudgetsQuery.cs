using MediatR;
using CompanyService.Core.DTOs.Finance;

namespace CompanyService.Core.Feature.Querys.Finance
{
    /// <summary>
    /// Query para obtener la lista de presupuestos
    /// </summary>
    public class GetBudgetsQuery : IRequest<List<BudgetResponseDto>>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
    }
}