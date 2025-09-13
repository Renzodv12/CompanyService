using CompanyService.Core.Entities;

namespace CompanyService.Core.Interfaces
{
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        Task<IEnumerable<Warehouse>> GetWarehousesByCompanyAsync(Guid companyId);
        Task<Warehouse?> GetWarehouseByIdAndCompanyAsync(Guid id, Guid companyId);
        Task<bool> ExistsWarehouseByNameAndCompanyAsync(string name, Guid companyId, Guid? excludeId = null);
        Task<IEnumerable<Warehouse>> GetActiveWarehousesByCompanyAsync(Guid companyId);
    }
}