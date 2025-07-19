using CompanyService.Core.Models.Report;

namespace CompanyService.Core.Interfaces
{
    public interface IReportService
    {
        Task<SalesReportDto> GenerateSalesReportAsync(Guid companyId, DateTime fromDate, DateTime toDate);
        Task<InventoryReportDto> GenerateInventoryReportAsync(Guid companyId);
    }
}
