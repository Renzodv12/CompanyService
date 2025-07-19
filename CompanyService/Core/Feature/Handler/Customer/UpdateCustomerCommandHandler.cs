using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Customer;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Customer
{
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Obtener el cliente
            var customer = await _unitOfWork.Repository<Entities.Customer>()
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId);

            if (customer == null)
                throw new DefaultException("Cliente no encontrado.");

            // Actualizar cliente
            customer.Name = request.Name;
            customer.Email = request.Email;
            customer.Phone = request.Phone;
            customer.Address = request.Address;
            customer.City = request.City;
            customer.IsActive = request.IsActive;
            customer.LastModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Entities.Customer>().Update(customer);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
