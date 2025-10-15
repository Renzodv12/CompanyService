using MediatR;
using CompanyService.Core.DTOs.Finance;

namespace CompanyService.Core.Feature.Querys.Finance
{
    /// <summary>
    /// Query para obtener el resumen financiero de ventas
    /// </summary>
    public class GetSalesFinancialSummaryQuery : IRequest<SalesFinancialSummaryDto>
    {
        public Guid CompanyId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
