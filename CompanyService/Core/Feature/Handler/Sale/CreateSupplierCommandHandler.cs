using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Sale;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Sale
{
    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSupplierCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Validar que no exista proveedor con mismo documento
            var existingSupplier = await _unitOfWork.Repository<CompanyService.Core.Entities.Supplier>()
                .FirstOrDefaultAsync(s => s.DocumentNumber == request.DocumentNumber && s.CompanyId == request.CompanyId);

            if (existingSupplier != null)
                throw new DefaultException($"Ya existe un proveedor con documento {request.DocumentNumber}.");

            var supplier = new CompanyService.Core.Entities.Supplier
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                DocumentNumber = request.DocumentNumber,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                City = request.City,
                ContactPerson = request.ContactPerson,
                IsActive = true,
                CompanyId = request.CompanyId,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CompanyService.Core.Entities.Supplier>().AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            return supplier.Id;
        }
    }
}
