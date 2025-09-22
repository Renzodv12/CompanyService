using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.Finance
{
    /// <summary>
    /// DTO for Cash Flow Report
    /// </summary>
    public class CashFlowReportDto
    {
        public Guid CompanyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalInflow { get; set; }
        public decimal TotalOutflow { get; set; }
        public decimal NetCashFlow { get; set; }
        public decimal OperatingCashFlow { get; set; }
        public decimal InvestingCashFlow { get; set; }
        public decimal FinancingCashFlow { get; set; }
        public List<CashFlowActivityDto> OperatingActivities { get; set; } = new();
        public List<CashFlowActivityDto> InvestingActivities { get; set; } = new();
        public List<CashFlowActivityDto> FinancingActivities { get; set; } = new();
        public List<CashFlowResponseDto> CashFlows { get; set; } = new();
        public int TransactionCount { get; set; }
    }

    public class CashFlowActivityDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Inflows { get; set; }
        public decimal Outflows { get; set; }
        public decimal NetAmount { get; set; }
        public int TransactionCount { get; set; }
    }
}