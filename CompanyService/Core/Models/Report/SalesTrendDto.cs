namespace CompanyService.Core.Models.Report
{
    public class SalesTrendDto
    {
        public string Period { get; set; } = string.Empty; // "2024-01", "2024-02", etc.
        public decimal Amount { get; set; }
        public int Count { get; set; }
        public decimal GrowthPercentage { get; set; }
    }
}
