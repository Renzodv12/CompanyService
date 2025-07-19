using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Customer;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Customer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Validar que no exista cliente con mismo documento
            var existingCustomer = await _unitOfWork.Repository<CompanyService.Core.Entities.Customer>()
                .FirstOrDefaultAsync(c => c.DocumentNumber == request.DocumentNumber && c.CompanyId == request.CompanyId);

            if (existingCustomer != null)
                throw new DefaultException($"Ya existe un cliente con documento {request.DocumentNumber}.");

            var customer = new CompanyService.Core.Entities.Customer
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                DocumentNumber = request.DocumentNumber,
                DocumentType = request.DocumentType,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                City = request.City,
                IsActive = true,
                CompanyId = request.CompanyId,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CompanyService.Core.Entities.Customer>().AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return customer.Id;
        }
    }
}
