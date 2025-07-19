using CompanyService.Core.Models.Product;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Product
{
    public class GetProductCategoriesQuery : IRequest<List<ProductCategoryDto>>
    {
        public Guid CompanyId { get; set; }
        public bool? IsActive { get; set; }
    }
}
