namespace CompanyService.Core.DTOs.Finance
{
    /// <summary>
    /// DTO for Balance Sheet Report
    /// </summary>
    public class BalanceSheetReportDto
    {
        public Guid CompanyId { get; set; }
        public DateTime AsOfDate { get; set; }
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal TotalEquity { get; set; }
        public List<AssetLineItem> Assets { get; set; } = new();
        public List<LiabilityLineItem> Liabilities { get; set; } = new();
        public List<EquityLineItem> Equity { get; set; } = new();
    }

    public class AssetLineItem
    {
        public string Category { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsCurrentAsset { get; set; }
    }

    public class LiabilityLineItem
    {
        public string Category { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsCurrentLiability { get; set; }
    }

    public class EquityLineItem
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}