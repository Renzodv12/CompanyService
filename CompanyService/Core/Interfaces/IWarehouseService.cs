using CompanyService.Core.DTOs;

namespace CompanyService.Core.Interfaces
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseResponseDto>> GetWarehousesByCompanyAsync(Guid companyId);
        Task<WarehouseResponseDto?> GetWarehouseByIdAsync(Guid id);
        Task<WarehouseResponseDto> CreateWarehouseAsync(CreateWarehouseDto createWarehouseDto);
        Task<WarehouseResponseDto?> UpdateWarehouseAsync(Guid id, UpdateWarehouseDto updateWarehouseDto);
        Task<bool> DeleteWarehouseAsync(Guid id);
        Task<bool> ActivateWarehouseAsync(Guid id);
        Task<bool> DeactivateWarehouseAsync(Guid id);
        Task<IEnumerable<WarehouseResponseDto>> GetActiveWarehousesByCompanyAsync(Guid companyId);
        Task<object> GetWarehouseStatsAsync(Guid warehouseId);
        Task<bool> WarehouseExistsAsync(Guid id);
        Task<bool> WarehouseBelongsToCompanyAsync(Guid warehouseId, Guid companyId);
    }
}