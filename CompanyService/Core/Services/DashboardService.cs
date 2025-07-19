using CompanyService.Core.Entities;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Report;

namespace CompanyService.Core.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid companyId)
        {
            var today = DateTime.UtcNow.Date;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var lastMonth = thisMonth.AddMonths(-1);

            // Ventas del mes actual
            var salesThisMonth = await _unitOfWork.Repository<Sale>()
                .WhereAsync(s => s.CompanyId == companyId && s.SaleDate >= thisMonth);

            var salesLastMonth = await _unitOfWork.Repository<Sale>()
                .WhereAsync(s => s.CompanyId == companyId && s.SaleDate >= lastMonth && s.SaleDate < thisMonth);

            // Productos con stock bajo
            var lowStockProducts = await _unitOfWork.Repository<Product>()
                .WhereAsync(p => p.CompanyId == companyId && p.IsActive && p.Stock <= p.MinStock);

            // Clientes activos
            var activeCustomers = await _unitOfWork.Repository<Customer>()
                .WhereAsync(c => c.CompanyId == companyId && c.IsActive);

            // Productos activos
            var activeProducts = await _unitOfWork.Repository<Product>()
                .WhereAsync(p => p.CompanyId == companyId && p.IsActive);

            var totalSalesThisMonth = salesThisMonth.Sum(s => s.TotalAmount);
            var totalSalesLastMonth = salesLastMonth.Sum(s => s.TotalAmount);
            var salesGrowth = totalSalesLastMonth > 0
                ? ((totalSalesThisMonth - totalSalesLastMonth) / totalSalesLastMonth) * 100
                : 0;

            return new DashboardSummaryDto
            {
                TotalSalesThisMonth = totalSalesThisMonth,
                SalesGrowthPercentage = salesGrowth,
                TotalCustomers = activeCustomers.Count(),
                TotalProducts = activeProducts.Count(),
                LowStockProductsCount = lowStockProducts.Count(),
                SalesCountThisMonth = salesThisMonth.Count(),
                RecentSales = salesThisMonth
                    .OrderByDescending(s => s.SaleDate)
                    .Take(5)
                    .Select(s => new RecentSaleDto
                    {
                        SaleNumber = s.SaleNumber,
                        CustomerName = activeCustomers.FirstOrDefault(c => c.Id == s.CustomerId)?.Name ?? "",
                        Amount = s.TotalAmount,
                        Date = s.SaleDate
                    }).ToList(),
                LowStockProducts = lowStockProducts
                    .Take(5)
                    .Select(p => new LowStockProductDto
                    {
                        Name = p.Name,
                        SKU = p.SKU,
                        CurrentStock = p.Stock,
                        MinStock = p.MinStock
                    }).ToList()
            };
        }
    }
}
