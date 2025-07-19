using CompanyService.Core.Feature.Querys.Customer;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Customer;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Customer
{
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCustomerByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.Repository<Entities.Customer>()
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId);

            if (customer == null)
                return null;

            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                DocumentNumber = customer.DocumentNumber,
                DocumentType = customer.DocumentType,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                City = customer.City,
                IsActive = customer.IsActive
            };
        }
    }
}
