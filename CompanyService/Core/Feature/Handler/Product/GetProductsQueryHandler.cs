using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Product;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Product;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Product
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.Repository<CompanyService.Core.Entities.Product>()
                .WhereAsync(p => p.CompanyId == request.CompanyId);

            var categories = await _unitOfWork.Repository<ProductCategory>()
                .WhereAsync(c => c.CompanyId == request.CompanyId);

            var query = products.AsQueryable();

            // Filtros
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(request.SearchTerm) ||
                                        p.SKU.Contains(request.SearchTerm) ||
                                        p.Barcode.Contains(request.SearchTerm));
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == request.IsActive.Value);
            }

            // Paginación
            var skip = (request.Page - 1) * request.PageSize;
            var pagedProducts = query.Skip(skip).Take(request.PageSize).ToList();

            return pagedProducts.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                SKU = p.SKU,
                Barcode = p.Barcode,
                Price = p.Price,
                Cost = p.Cost,
                Stock = p.Stock,
                MinStock = p.MinStock,
                MaxStock = p.MaxStock,
                IsActive = p.IsActive,
                Type = p.Type,
                Unit = p.Unit,
                CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name ?? ""
            }).ToList();
        }
    }
}
