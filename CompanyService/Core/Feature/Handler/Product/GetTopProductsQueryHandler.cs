using CompanyService.Core.Entities;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Product;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Product
{
    public class GetTopProductsQueryHandler : IRequestHandler<GetTopProductsQuery, List<TopProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTopProductsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TopProductDto>> Handle(GetTopProductsQuery request, CancellationToken cancellationToken)
        {
            var sales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.CompanyId == request.CompanyId &&
                               s.SaleDate >= request.FromDate &&
                               s.SaleDate <= request.ToDate);

            var saleIds = sales.Select(s => s.Id).ToList();
            var saleDetails = await _unitOfWork.Repository<SaleDetail>()
                .WhereAsync(sd => saleIds.Contains(sd.SaleId));

            var products = await _unitOfWork.Repository<CompanyService.Core.Entities.Product>()
                .WhereAsync(p => p.CompanyId == request.CompanyId);

            var topProducts = saleDetails
                .GroupBy(sd => sd.ProductId)
                .Select(g => new TopProductDto
                {
                    ProductName = products.FirstOrDefault(p => p.Id == g.Key)?.Name ?? "Producto no encontrado",
                    SKU = products.FirstOrDefault(p => p.Id == g.Key)?.SKU ?? "",
                    TotalQuantitySold = g.Sum(sd => sd.Quantity),
                    TotalRevenue = g.Sum(sd => sd.Subtotal),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(tp => tp.TotalQuantitySold)
                .Take(request.Top)
                .ToList();

            return topProducts;
        }
    }
}
