using CompanyService.Core.Entities;

namespace CompanyService.Core.Interfaces
{
    public interface IBatchRepository : IRepository<Batch>
    {
        Task<IEnumerable<Batch>> GetBatchesByCompanyAsync(Guid companyId);
        Task<IEnumerable<Batch>> GetBatchesByProductAsync(Guid productId, Guid companyId);
        Task<IEnumerable<Batch>> GetBatchesByWarehouseAsync(Guid warehouseId, Guid companyId);
        Task<Batch?> GetBatchByIdAndCompanyAsync(Guid id, Guid companyId);
        Task<IEnumerable<Batch>> GetExpiredBatchesAsync(Guid companyId, DateTime? asOfDate = null);
        Task<IEnumerable<Batch>> GetBatchesExpiringInDaysAsync(Guid companyId, int days);
        Task<bool> ExistsBatchByNumberAndCompanyAsync(string batchNumber, Guid companyId, Guid? excludeId = null);
        Task<decimal> GetAvailableQuantityByProductAsync(Guid productId, Guid warehouseId, Guid companyId);
        Task<IEnumerable<Batch>> GetBatchesWithLowStockAsync(Guid companyId, decimal minimumQuantity);
    }
}