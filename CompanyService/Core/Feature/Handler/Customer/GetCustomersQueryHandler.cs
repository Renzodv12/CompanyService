using CompanyService.Core.Feature.Querys.Customer;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Customer;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Customer
{
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<CustomerDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCustomersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = await _unitOfWork.Repository<Entities.Customer>()
                .WhereAsync(c => c.CompanyId == request.CompanyId);

            var query = customers.AsQueryable();

            // Filtro de búsqueda
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(c => c.Name.Contains(request.SearchTerm) ||
                                        c.DocumentNumber.Contains(request.SearchTerm) ||
                                        c.Email.Contains(request.SearchTerm));
            }

            // Ordenar por nombre
            query = query.OrderBy(c => c.Name);

            // Paginación
            var skip = (request.Page - 1) * request.PageSize;
            var pagedCustomers = query.Skip(skip).Take(request.PageSize).ToList();

            return pagedCustomers.Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                DocumentNumber = c.DocumentNumber,
                DocumentType = c.DocumentType,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                City = c.City,
                IsActive = c.IsActive
            }).ToList();
        }
    }
}
