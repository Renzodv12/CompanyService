using CompanyService.Core.DTOs;

namespace CompanyService.Core.Interfaces
{
    public interface IBatchService
    {
        Task<IEnumerable<BatchResponseDto>> GetBatchesByCompanyAsync(Guid companyId);
        Task<IEnumerable<BatchResponseDto>> GetBatchesByWarehouseAsync(Guid warehouseId);
        Task<IEnumerable<BatchResponseDto>> GetBatchesByProductAsync(Guid productId);
        Task<BatchResponseDto?> GetBatchByIdAsync(Guid id);
        Task<BatchResponseDto> CreateBatchAsync(CreateBatchDto createBatchDto);
        Task<BatchResponseDto?> UpdateBatchAsync(Guid id, UpdateBatchDto updateBatchDto);
        Task<bool> DeleteBatchAsync(Guid id);
        Task<bool> MoveBatchQuantityAsync(BatchMovementDto movementDto);
        Task<IEnumerable<BatchResponseDto>> GetExpiredBatchesAsync(Guid companyId);
        Task<IEnumerable<BatchResponseDto>> GetNearExpirationBatchesAsync(Guid companyId, int days = 30);
        Task<IEnumerable<BatchResponseDto>> GetLowStockBatchesAsync(Guid companyId, decimal threshold = 10);
        Task<object> GetBatchStatsAsync(Guid companyId);
        Task<decimal> GetAvailableQuantityAsync(Guid productId, Guid warehouseId);
        Task<bool> BatchExistsAsync(Guid id);
        Task<bool> BatchBelongsToCompanyAsync(Guid batchId, Guid companyId);
        Task<bool> HasSufficientQuantityAsync(Guid batchId, decimal quantity);
    }
}