using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Product;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Product;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Product
{
    public class GetLowStockProductsQueryHandler : IRequestHandler<GetLowStockProductsQuery, List<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLowStockProductsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProductDto>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.Repository<CompanyService.Core.Entities.Product>()
                .WhereAsync(p => p.CompanyId == request.CompanyId &&
                               p.IsActive &&
                               p.Stock <= p.MinStock);

            var categories = await _unitOfWork.Repository<ProductCategory>()
                .WhereAsync(c => c.CompanyId == request.CompanyId);

            return products.Select(p => new ProductDto
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
