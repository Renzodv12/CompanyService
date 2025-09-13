using CompanyService.Core.Entities;

namespace CompanyService.Core.Interfaces
{
    public interface IPhysicalInventoryRepository : IRepository<PhysicalInventory>
    {
        Task<IEnumerable<PhysicalInventory>> GetPhysicalInventoriesByCompanyAsync(Guid companyId);
        Task<IEnumerable<PhysicalInventory>> GetPhysicalInventoriesByWarehouseAsync(Guid warehouseId, Guid companyId);
        Task<PhysicalInventory?> GetPhysicalInventoryByIdAndCompanyAsync(Guid id, Guid companyId);
        Task<PhysicalInventory?> GetPhysicalInventoryWithItemsAsync(Guid id, Guid companyId);
        Task<IEnumerable<PhysicalInventory>> GetPhysicalInventoriesByStatusAsync(Guid companyId, PhysicalInventoryStatus status);
        Task<IEnumerable<PhysicalInventory>> GetPhysicalInventoriesByDateRangeAsync(Guid companyId, DateTime startDate, DateTime endDate);
        Task<bool> ExistsActivePhysicalInventoryForWarehouseAsync(Guid warehouseId, Guid companyId);
        Task<bool> ExistsPhysicalInventoryByNumberAndCompanyAsync(string inventoryNumber, Guid companyId, Guid? excludeId = null);
    }
}