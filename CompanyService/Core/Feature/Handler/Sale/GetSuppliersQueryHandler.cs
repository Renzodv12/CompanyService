using CompanyService.Core.Feature.Querys.Sale;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Sale;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Sale
{
    public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, List<SupplierDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSuppliersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SupplierDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
        {
            var suppliers = await _unitOfWork.Repository<CompanyService.Core.Entities.Supplier>()
                .WhereAsync(s => s.CompanyId == request.CompanyId);

            var query = suppliers.AsQueryable();

            // Filtro de búsqueda
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(s => s.Name.Contains(request.SearchTerm) ||
                                        s.DocumentNumber.Contains(request.SearchTerm) ||
                                        s.Email.Contains(request.SearchTerm));
            }

            // Ordenar por nombre
            query = query.OrderBy(s => s.Name);

            // Paginación
            var skip = (request.Page - 1) * request.PageSize;
            var pagedSuppliers = query.Skip(skip).Take(request.PageSize).ToList();

            return pagedSuppliers.Select(s => new SupplierDto
            {
                Id = s.Id,
                Name = s.Name,
                DocumentNumber = s.DocumentNumber,
                Email = s.Email,
                Phone = s.Phone,
                Address = s.Address,
                City = s.City,
                ContactPerson = s.ContactPerson,
                IsActive = s.IsActive
            }).ToList();
        }
    }
}

