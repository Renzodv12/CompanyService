using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Product;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Product
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Obtener el producto
            var product = await _unitOfWork.Repository<CompanyService.Core.Entities.Product>()
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.CompanyId == request.CompanyId);

            if (product == null)
                throw new DefaultException("Producto no encontrado.");

            // Validar que la categoría existe
            var category = await _unitOfWork.Repository<ProductCategory>()
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.CompanyId == request.CompanyId);

            if (category == null)
                throw new DefaultException("La categoría especificada no existe.");

            // Validaciones de negocio
            if (request.Price <= 0)
                throw new DefaultException("El precio debe ser mayor a cero.");
            
            if (request.Cost < 0)
                throw new DefaultException("El costo no puede ser negativo.");
            
            if (request.Cost > request.Price)
                throw new DefaultException("El costo no puede ser mayor al precio (generaría pérdida).");
            
            if (request.MinStock < 0)
                throw new DefaultException("El stock mínimo no puede ser negativo.");
            
            if (request.MaxStock < 0)
                throw new DefaultException("El stock máximo no puede ser negativo.");
            
            if (request.MinStock > request.MaxStock)
                throw new DefaultException("El stock mínimo no puede ser mayor al stock máximo.");

            // Actualizar producto
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Cost = request.Cost;
            product.MinStock = request.MinStock;
            product.MaxStock = request.MaxStock;
            product.Unit = request.Unit;
            product.Weight = request.Weight;
            product.CategoryId = request.CategoryId;
            product.IsActive = request.IsActive;
            
            // Actualizar campos opcionales solo si se proporcionan
            if (request.Barcode != null)
                product.Barcode = request.Barcode;
            
            if (request.ImageUrl != null)
                product.ImageUrl = request.ImageUrl;
            
            product.LastModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<CompanyService.Core.Entities.Product>().Update(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
