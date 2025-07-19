using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Product;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Product
{
    public class CreateProductCategoryCommandHandler : IRequestHandler<CreateProductCategoryCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Validar que no exista categoría con mismo nombre
            var existingCategory = await _unitOfWork.Repository<CompanyService.Core.Entities.ProductCategory>()
                .FirstOrDefaultAsync(c => c.Name == request.Name && c.CompanyId == request.CompanyId);

            if (existingCategory != null)
                throw new DefaultException($"Ya existe una categoría con el nombre '{request.Name}'.");

            var category = new CompanyService.Core.Entities.ProductCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                CompanyId = request.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CompanyService.Core.Entities.ProductCategory>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return category.Id;
        }
    }
}
