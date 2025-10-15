using MediatR;
using CompanyService.Core.DTOs.Finance;

namespace CompanyService.Core.Feature.Querys.Finance
{
    /// <summary>
    /// Query para obtener el resumen de presupuestos
    /// </summary>
    public class GetBudgetSummaryQuery : IRequest<BudgetSummaryDto>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
