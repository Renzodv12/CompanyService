using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Product;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Product;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Product
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductDetailDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Repository<CompanyService.Core.Entities.Product>()
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.CompanyId == request.CompanyId);

            if (product == null)
                return null;

            var category = await _unitOfWork.Repository<ProductCategory>()
                .FirstOrDefaultAsync(c => c.Id == product.CategoryId);

            var recentMovements = await _unitOfWork.Repository<StockMovement>()
                .WhereAsync(sm => sm.ProductId == request.Id);

            var movements = recentMovements
                .OrderByDescending(sm => sm.CreatedAt)
                .Take(10)
                .Select(sm => new StockMovementDto
                {
                    Date = sm.CreatedAt,
                    Type = sm.Type,
                    Quantity = sm.Quantity,
                    PreviousStock = sm.PreviousStock,
                    NewStock = sm.NewStock,
                    Reason = sm.Reason,
                    Reference = sm.Reference
                }).ToList();

            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                Barcode = product.Barcode,
                Price = product.Price,
                Cost = product.Cost,
                Stock = product.Stock,
                MinStock = product.MinStock,
                MaxStock = product.MaxStock,
                IsActive = product.IsActive,
                Type = product.Type,
                Unit = product.Unit,
                Weight = product.Weight,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = category?.Name ?? "",
                CreatedAt = product.CreatedAt,
                LastModifiedAt = product.LastModifiedAt,
                RecentMovements = movements
            };
        }
    }
}
