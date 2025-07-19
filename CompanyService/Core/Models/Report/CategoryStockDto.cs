namespace CompanyService.Core.Models.Report
{
    public class CategoryStockDto
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalStockValue { get; set; }
    }
}
