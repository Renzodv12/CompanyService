namespace CompanyService.Core.Models.Report
{
    public class RevenueTrendDto
    {
        public string Period { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
    }
}
