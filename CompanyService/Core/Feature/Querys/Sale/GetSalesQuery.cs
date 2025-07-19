using CompanyService.Core.Models.Sale;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Sale
{
    public class GetSalesQuery : IRequest<List<SaleDto>>
    {
        public Guid CompanyId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
