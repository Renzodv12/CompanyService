using CompanyService.Core.Models.Sale;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Sale
{
    public class GetSaleByIdQuery : IRequest<SaleDetailDto?>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
    }
}
