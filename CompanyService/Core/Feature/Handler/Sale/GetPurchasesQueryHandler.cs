using CompanyService.Core.Feature.Querys.Sale;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Sale;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Sale
{
    public class GetPurchasesQueryHandler : IRequestHandler<GetPurchasesQuery, List<PurchaseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPurchasesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PurchaseDto>> Handle(GetPurchasesQuery request, CancellationToken cancellationToken)
        {
            var purchases = await _unitOfWork.Repository<CompanyService.Core.Entities.Purchase>()
                .WhereAsync(p => p.CompanyId == request.CompanyId);

            var suppliers = await _unitOfWork.Repository<CompanyService.Core.Entities.Supplier>()
                .WhereAsync(s => s.CompanyId == request.CompanyId);

            var query = purchases.AsQueryable();

            // Filtros de fecha
            if (request.FromDate.HasValue)
                query = query.Where(p => p.PurchaseDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(p => p.PurchaseDate <= request.ToDate.Value);

            // Ordenar por fecha descendente
            query = query.OrderByDescending(p => p.PurchaseDate);

            // Paginación
            var skip = (request.Page - 1) * request.PageSize;
            var pagedPurchases = query.Skip(skip).Take(request.PageSize).ToList();

            return pagedPurchases.Select(p => new PurchaseDto
            {
                Id = p.Id,
                PurchaseNumber = p.PurchaseNumber,
                SupplierName = suppliers.FirstOrDefault(s => s.Id == p.SupplierId)?.Name ?? "",
                PurchaseDate = p.PurchaseDate,
                DeliveryDate = p.DeliveryDate,
                TotalAmount = p.TotalAmount,
                Status = p.Status,
                InvoiceNumber = p.InvoiceNumber
            }).ToList();
        }
    }
}
