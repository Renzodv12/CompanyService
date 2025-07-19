using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Product;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Product
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Validar que la empresa existe y el usuario tiene acceso
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Validar que el SKU no exista en la empresa
            var existingProduct = await _unitOfWork.Repository<CompanyService.Core.Entities.Product>()
                .FirstOrDefaultAsync(p => p.SKU == request.SKU && p.CompanyId == request.CompanyId);

            if (existingProduct != null)
                throw new DefaultException($"Ya existe un producto con SKU '{request.SKU}'.");

            // Validar que la categoría existe
            var category = await _unitOfWork.Repository<ProductCategory>()
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.CompanyId == request.CompanyId);

            if (category == null)
                throw new DefaultException("La categoría especificada no existe.");

            var product = new CompanyService.Core.Entities.Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                SKU = request.SKU,
                Barcode = request.Barcode,
                Price = request.Price,
                Cost = request.Cost,
                Stock = request.Stock,
                MinStock = request.MinStock,
                MaxStock = request.MaxStock,
                Unit = request.Unit,
                Weight = request.Weight,
                CategoryId = request.CategoryId,
                CompanyId = request.CompanyId,
                Type = ProductType.Product,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CompanyService.Core.Entities.Product>().AddAsync(product);

            // Crear movimiento de stock inicial si hay stock
            if (request.Stock > 0)
            {
                var stockMovement = new StockMovement
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Type = MovementType.Entry,
                    Quantity = request.Stock,
                    PreviousStock = 0,
                    NewStock = request.Stock,
                    Reason = "Stock inicial",
                    Reference = "PRODUCT_CREATION",
                    UserId = Guid.Parse(request.UserId),
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<StockMovement>().AddAsync(stockMovement);
            }

            await _unitOfWork.SaveChangesAsync();
            return product.Id;
        }
    }
}
