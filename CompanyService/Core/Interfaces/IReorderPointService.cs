using CompanyService.Core.DTOs;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Interfaces
{
    public interface IReorderPointService
    {
        Task<IEnumerable<ReorderPointResponseDto>> GetReorderPointsByCompanyAsync(Guid companyId);
        Task<IEnumerable<ReorderPointResponseDto>> GetReorderPointsByWarehouseAsync(Guid warehouseId);
        Task<IEnumerable<ReorderPointResponseDto>> GetReorderPointsByProductAsync(Guid productId);
        Task<ReorderPointResponseDto?> GetReorderPointByIdAsync(Guid id);
        Task<ReorderPointResponseDto> CreateReorderPointAsync(CreateReorderPointDto createDto);
        Task<ReorderPointResponseDto?> UpdateReorderPointAsync(Guid id, UpdateReorderPointDto updateDto);
        Task<bool> DeleteReorderPointAsync(Guid id);
        Task<IEnumerable<ReorderPointResponseDto>> GetTriggeredReorderPointsAsync(Guid companyId);
        Task<IEnumerable<ReorderAlertDto>> GetActiveReorderAlertsAsync(Guid companyId);
        Task<bool> TriggerReorderPointAsync(Guid reorderPointId, decimal currentQuantity);
        Task<bool> ResolveReorderAlertAsync(Guid alertId, ResolveReorderAlertDto resolveDto);
        Task<bool> MarkAsOrderedAsync(Guid reorderPointId);
        Task<object> GetReorderStatsAsync(Guid companyId);
        Task<IEnumerable<ReorderPointResponseDto>> GetReorderPointsByStatusAsync(Guid companyId, ReorderStatus status);
        Task<bool> CheckAndTriggerReorderPointsAsync(Guid companyId);
        Task<bool> ReorderPointExistsAsync(Guid id);
        Task<bool> ReorderPointBelongsToCompanyAsync(Guid reorderPointId, Guid companyId);
        Task<bool> ReorderPointExistsForProductWarehouseAsync(Guid productId, Guid warehouseId);
        Task<decimal> CalculateOptimalReorderQuantityAsync(Guid productId, Guid warehouseId);
    }
}