namespace CompanyService.Core.Models.Report
{
    public class DashboardSummaryDto
    {
        // Sales Metrics
        public decimal TotalSalesThisMonth { get; set; }
        public decimal TotalSalesLastMonth { get; set; }
        public decimal SalesGrowthPercentage { get; set; }
        public int SalesCountThisMonth { get; set; }
        public int SalesCountLastMonth { get; set; }
        public decimal AverageSaleAmount { get; set; }
        
        // Customer Metrics
        public int TotalCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int ActiveCustomersThisMonth { get; set; }
        
        // Product & Inventory Metrics
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int LowStockProductsCount { get; set; }
        public int OutOfStockProductsCount { get; set; }
        public decimal TotalInventoryValue { get; set; }
        
        // Financial Metrics
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        
        // Budget Metrics
        public int TotalBudgets { get; set; }
        public int ActiveBudgets { get; set; }
        public decimal TotalBudgetedAmount { get; set; }
        public decimal TotalActualAmount { get; set; }
        public decimal BudgetVariance { get; set; }
        
        // CRM Metrics
        public int TotalLeads { get; set; }
        public int NewLeadsThisMonth { get; set; }
        public int ConvertedLeadsThisMonth { get; set; }
        public decimal LeadConversionRate { get; set; }
        
        // Recent Activity
        public List<RecentSaleDto> RecentSales { get; set; } = new();
        public List<LowStockProductDto> LowStockProducts { get; set; } = new();
        public List<RecentLeadDto> RecentLeads { get; set; } = new();
        public List<RecentBudgetDto> RecentBudgets { get; set; } = new();
        
        // Charts & Trends
        public List<SalesTrendDto> SalesTrend { get; set; } = new();
        public List<RevenueTrendDto> RevenueTrend { get; set; } = new();
        public List<CustomerGrowthDto> CustomerGrowth { get; set; } = new();
        
        // Performance Indicators
        public List<KPIDto> KPIs { get; set; } = new();
        public List<AlertDto> Alerts { get; set; } = new();
    }
}
