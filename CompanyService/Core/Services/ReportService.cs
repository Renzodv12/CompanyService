using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Report;

namespace CompanyService.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SalesReportDto> GenerateSalesReportAsync(Guid companyId, DateTime fromDate, DateTime toDate)
        {
            var sales = await _unitOfWork.Repository<Sale>()
                .WhereAsync(s => s.CompanyId == companyId &&
                               s.SaleDate >= fromDate &&
                               s.SaleDate <= toDate &&
                               s.Status == SaleStatus.Completed);

            var customers = await _unitOfWork.Repository<Customer>()
                .WhereAsync(c => c.CompanyId == companyId);

            var totalSales = sales.Sum(s => s.TotalAmount);
            var totalTransactions = sales.Count();
            var averageTicket = totalTransactions > 0 ? totalSales / totalTransactions : 0;

            var salesByDay = sales
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => new DailySalesDto
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(s => s.TotalAmount),
                    TransactionCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            var topCustomers = sales
                .GroupBy(s => s.CustomerId)
                .Select(g => new TopCustomerDto
                {
                    CustomerName = customers.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "Cliente no encontrado",
                    TotalAmount = g.Sum(s => s.TotalAmount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(c => c.TotalAmount)
                .Take(10)
                .ToList();

            return new SalesReportDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalSales = totalSales,
                TotalTransactions = totalTransactions,
                AverageTicket = averageTicket,
                SalesByDay = salesByDay,
                TopCustomers = topCustomers
            };
        }

        public async Task<InventoryReportDto> GenerateInventoryReportAsync(Guid companyId)
        {
            var products = await _unitOfWork.Repository<Product>()
                .WhereAsync(p => p.CompanyId == companyId && p.IsActive);

            var categories = await _unitOfWork.Repository<ProductCategory>()
                .WhereAsync(c => c.CompanyId == companyId);

            var totalProducts = products.Count();
            var totalStockValue = products.Sum(p => p.Stock * p.Cost);
            var lowStockProducts = products.Where(p => p.Stock <= p.MinStock).Count();

            var productsByCategory = products
                .GroupBy(p => p.CategoryId)
                .Select(g => new CategoryStockDto
                {
                    CategoryName = categories.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "Sin categoría",
                    ProductCount = g.Count(),
                    TotalStockValue = g.Sum(p => p.Stock * p.Cost)
                })
                .OrderByDescending(c => c.TotalStockValue)
                .ToList();

            return new InventoryReportDto
            {
                TotalProducts = totalProducts,
                TotalStockValue = totalStockValue,
                LowStockProductsCount = lowStockProducts,
                ProductsByCategory = productsByCategory,
                LowStockProducts = products
                    .Where(p => p.Stock <= p.MinStock)
                    .Select(p => new LowStockProductDto
                    {
                        Name = p.Name,
                        SKU = p.SKU,
                        CurrentStock = p.Stock,
                        MinStock = p.MinStock
                    })
                    .ToList()
            };
        }
    }
}
