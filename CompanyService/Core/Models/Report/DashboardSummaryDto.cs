namespace CompanyService.Core.Models.Report
{
    public class DashboardSummaryDto
    {
        public decimal TotalSalesThisMonth { get; set; }
        public decimal SalesGrowthPercentage { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockProductsCount { get; set; }
        public int SalesCountThisMonth { get; set; }
        public List<RecentSaleDto> RecentSales { get; set; } = new();
        public List<LowStockProductDto> LowStockProducts { get; set; } = new();
    }
}
