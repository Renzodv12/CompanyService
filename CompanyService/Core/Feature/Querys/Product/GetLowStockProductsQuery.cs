using CompanyService.Core.Models.Product;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Product
{
    public class GetLowStockProductsQuery : IRequest<List<ProductDto>>
    {
        public Guid CompanyId { get; set; }
    }
}
