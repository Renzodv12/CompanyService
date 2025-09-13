using CompanyService.Core.Entities;

namespace CompanyService.Core.Interfaces
{
    public interface IReorderPointRepository : IRepository<ReorderPoint>
    {
        Task<IEnumerable<ReorderPoint>> GetReorderPointsByCompanyAsync(Guid companyId);
        Task<IEnumerable<ReorderPoint>> GetReorderPointsByWarehouseAsync(Guid warehouseId, Guid companyId);
        Task<ReorderPoint?> GetReorderPointByIdAndCompanyAsync(Guid id, Guid companyId);
        Task<ReorderPoint?> GetReorderPointByProductAndWarehouseAsync(Guid productId, Guid warehouseId, Guid companyId);
        Task<IEnumerable<ReorderPoint>> GetTriggeredReorderPointsAsync(Guid companyId);
        Task<IEnumerable<ReorderPoint>> GetReorderPointsWithAlertsAsync(Guid companyId);
        Task<bool> ExistsReorderPointForProductAndWarehouseAsync(Guid productId, Guid warehouseId, Guid companyId, Guid? excludeId = null);
        Task<IEnumerable<ReorderAlert>> GetActiveReorderAlertsAsync(Guid companyId);
        Task<IEnumerable<ReorderAlert>> GetReorderAlertsByReorderPointAsync(Guid reorderPointId);
    }
}