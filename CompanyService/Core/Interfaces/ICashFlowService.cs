using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Interfaces
{
    public interface ICashFlowService
    {
        // Cash Flow Management
        Task<CashFlowResponseDto> CreateCashFlowAsync(CreateCashFlowDto dto);
        Task<CashFlowResponseDto> GetCashFlowByIdAsync(Guid id);
        Task<IEnumerable<CashFlowResponseDto>> GetCashFlowsByCompanyAsync(Guid companyId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<CashFlowResponseDto>> GetCashFlowsByTypeAsync(Guid companyId, CashFlowType type, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<CashFlowResponseDto>> GetCashFlowsByCategoryAsync(Guid companyId, string category, DateTime? startDate = null, DateTime? endDate = null);
        Task<CashFlowResponseDto> UpdateCashFlowAsync(Guid id, CreateCashFlowDto dto);
        Task<bool> DeleteCashFlowAsync(Guid id);
        
        // Cash Flow Analysis
        Task<CashFlowSummaryDto> GetCashFlowSummaryAsync(Guid companyId, DateTime startDate, DateTime endDate);
        Task<CashFlowSummaryDto> GetMonthlyCashFlowSummaryAsync(Guid companyId, int year, int month);
        Task<CashFlowSummaryDto> GetYearlyCashFlowSummaryAsync(Guid companyId, int year);
        Task<IEnumerable<CashFlowSummaryDto>> GetCashFlowTrendAsync(Guid companyId, DateTime startDate, DateTime endDate, string period = "monthly");
        
        // Cash Flow Projections
        Task<object> GetCashFlowProjectionAsync(Guid companyId, DateTime projectionDate, int months = 12);
        Task<object> GetCashFlowForecastAsync(Guid companyId, DateTime startDate, DateTime endDate);
        
        // Cash Flow Reports
        Task<object> GetOperatingCashFlowReportAsync(Guid companyId, DateTime startDate, DateTime endDate);
        Task<object> GetInvestingCashFlowReportAsync(Guid companyId, DateTime startDate, DateTime endDate);
        Task<object> GetFinancingCashFlowReportAsync(Guid companyId, DateTime startDate, DateTime endDate);
        Task<object> GetCashFlowStatementAsync(Guid companyId, DateTime startDate, DateTime endDate);
        
        // Cash Flow Categories
        Task<IEnumerable<string>> GetCashFlowCategoriesAsync(Guid companyId);
        Task<object> GetCashFlowByCategoryReportAsync(Guid companyId, DateTime startDate, DateTime endDate);
        
        // Cash Position
        Task<decimal> GetCurrentCashPositionAsync(Guid companyId);
        Task<object> GetCashPositionHistoryAsync(Guid companyId, DateTime startDate, DateTime endDate);
        Task<object> GetCashBurnRateAsync(Guid companyId, DateTime startDate, DateTime endDate);
    }
}