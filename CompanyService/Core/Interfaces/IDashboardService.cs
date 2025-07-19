using CompanyService.Core.Models.Report;

namespace CompanyService.Core.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid companyId);
    }
}
