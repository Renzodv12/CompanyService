namespace CompanyService.Core.Models.Report
{
    public class InventoryReportDto
    {
        public int TotalProducts { get; set; }
        public decimal TotalStockValue { get; set; }
        public int LowStockProductsCount { get; set; }
        public List<CategoryStockDto> ProductsByCategory { get; set; } = new();
        public List<LowStockProductDto> LowStockProducts { get; set; } = new();
    }
}
