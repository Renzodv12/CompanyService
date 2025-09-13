using CompanyService.Core.DTOs;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Interfaces
{
    public interface IPhysicalInventoryService
    {
        Task<IEnumerable<PhysicalInventoryResponseDto>> GetPhysicalInventoriesByCompanyAsync(Guid companyId);
        Task<IEnumerable<PhysicalInventoryResponseDto>> GetPhysicalInventoriesByWarehouseAsync(Guid warehouseId);
        Task<PhysicalInventoryResponseDto?> GetPhysicalInventoryByIdAsync(Guid id);
        Task<PhysicalInventoryResponseDto> CreatePhysicalInventoryAsync(CreatePhysicalInventoryDto createDto);
        Task<PhysicalInventoryResponseDto?> UpdatePhysicalInventoryAsync(Guid id, UpdatePhysicalInventoryDto updateDto);
        Task<bool> DeletePhysicalInventoryAsync(Guid id);
        Task<bool> StartPhysicalInventoryAsync(Guid id);
        Task<bool> CompletePhysicalInventoryAsync(Guid id);
        Task<bool> CancelPhysicalInventoryAsync(Guid id);
        Task<PhysicalInventoryItemResponseDto?> UpdateInventoryItemAsync(Guid inventoryId, Guid itemId, UpdatePhysicalInventoryItemDto updateDto);
        Task<IEnumerable<PhysicalInventoryItemResponseDto>> GetInventoryItemsAsync(Guid inventoryId);
        Task<object> GetInventoryVarianceReportAsync(Guid inventoryId);
        Task<IEnumerable<PhysicalInventoryResponseDto>> GetInventoriesByStatusAsync(Guid companyId, PhysicalInventoryStatus status);
        Task<object> GetInventoryStatsAsync(Guid companyId);
        Task<bool> PhysicalInventoryExistsAsync(Guid id);
        Task<bool> PhysicalInventoryBelongsToCompanyAsync(Guid inventoryId, Guid companyId);
        Task<bool> CanModifyInventoryAsync(Guid inventoryId);
    }
}