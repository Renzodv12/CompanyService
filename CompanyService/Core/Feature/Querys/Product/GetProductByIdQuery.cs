using CompanyService.Core.Models.Product;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Product
{
    public class GetProductByIdQuery : IRequest<ProductDetailDto?>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
    }
}
