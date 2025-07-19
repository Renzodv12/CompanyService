using CompanyService.Core.Models.Product;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Product
{
    public class GetProductsQuery : IRequest<List<ProductDto>>
    {
        public Guid CompanyId { get; set; }
        public string SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
