using CompanyService.Core.Feature.Querys.Sale;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Sale;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Sale
{
    public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, List<SaleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSalesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SaleDto>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
        {
            var sales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.CompanyId == request.CompanyId);

            var customers = await _unitOfWork.Repository<CompanyService.Core.Entities.Customer>()
                .WhereAsync(c => c.CompanyId == request.CompanyId);

            var query = sales.AsQueryable();

            // Filtros de fecha
            if (request.FromDate.HasValue)
                query = query.Where(s => s.SaleDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(s => s.SaleDate <= request.ToDate.Value);

            // Ordenar por fecha descendente
            query = query.OrderByDescending(s => s.SaleDate);

            // Paginación
            var skip = (request.Page - 1) * request.PageSize;
            var pagedSales = query.Skip(skip).Take(request.PageSize).ToList();

            return pagedSales.Select(s => new SaleDto
            {
                Id = s.Id,
                SaleNumber = s.SaleNumber,
                CustomerName = customers.FirstOrDefault(c => c.Id == s.CustomerId)?.Name ?? "",
                SaleDate = s.SaleDate,
                TotalAmount = s.TotalAmount,
                Status = s.Status,
                PaymentMethod = s.PaymentMethod,
                IsElectronicInvoice = s.IsElectronicInvoice
            }).ToList();
        }
    }
}
