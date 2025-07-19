using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Product;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Product
{
    public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStockCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Obtener el producto
            var product = await _unitOfWork.Repository<CompanyService.Core.Entities.Product>()
                .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.CompanyId == request.CompanyId);

            if (product == null)
                throw new DefaultException("Producto no encontrado.");

            var previousStock = product.Stock;
            var newStock = previousStock;

            // Calcular nuevo stock según el tipo de movimiento
            switch (request.MovementType)
            {
                case Core.Enums.MovementType.Entry:
                    newStock += request.Quantity;
                    break;
                case Core.Enums.MovementType.Exit:
                    newStock -= request.Quantity;
                    if (newStock < 0)
                        throw new DefaultException("Stock insuficiente.");
                    break;
                case Core.Enums.MovementType.Adjustment:
                    newStock = request.Quantity;
                    break;
            }

            // Actualizar stock del producto
            product.Stock = newStock;
            product.LastModifiedAt = DateTime.UtcNow;
            _unitOfWork.Repository<CompanyService.Core.Entities.Product>().Update(product);

            // Crear movimiento de stock
            var stockMovement = new StockMovement
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Type = request.MovementType,
                Quantity = Math.Abs(request.Quantity),
                PreviousStock = previousStock,
                NewStock = newStock,
                Reason = request.Reason,
                Reference = request.Reference,
                UserId = Guid.Parse(request.UserId),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<StockMovement>().AddAsync(stockMovement);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
