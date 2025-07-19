using CompanyService.Core.Feature.Querys.Product;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Product;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Product
{
    public class GetProductCategoriesQueryHandler : IRequestHandler<GetProductCategoriesQuery, List<ProductCategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProductCategoryDto>> Handle(GetProductCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.Repository<CompanyService.Core.Entities.ProductCategory>()
                .WhereAsync(c => c.CompanyId == request.CompanyId);

            var query = categories.AsQueryable();

            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }

            return query.OrderBy(c => c.Name)
                .Select(c => new ProductCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                }).ToList();
        }
    }
}
