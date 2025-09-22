namespace CompanyService.Core.DTOs.Finance
{
    /// <summary>
    /// DTO for Profit and Loss Report
    /// </summary>
    public class ProfitLossReportDto
    {
        public Guid CompanyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public List<RevenueLineItem> RevenueItems { get; set; } = new();
        public List<ExpenseLineItem> ExpenseItems { get; set; } = new();
    }

    public class RevenueLineItem
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ExpenseLineItem
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }
}