using CompanyService.Core.Entities;
using CompanyService.Core.Interfaces;
using CompanyService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.Repositories
{
    public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Warehouse>> GetWarehousesByCompanyAsync(Guid companyId)
        {
            return await _context.Warehouses
                .Where(w => w.CompanyId == companyId)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }

        public async Task<Warehouse?> GetWarehouseByIdAndCompanyAsync(Guid id, Guid companyId)
        {
            return await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == id && w.CompanyId == companyId);
        }

        public async Task<bool> ExistsWarehouseByNameAndCompanyAsync(string name, Guid companyId, Guid? excludeId = null)
        {
            var query = _context.Warehouses
                .Where(w => w.Name.ToLower() == name.ToLower() && w.CompanyId == companyId);

            if (excludeId.HasValue)
            {
                query = query.Where(w => w.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetActiveWarehousesByCompanyAsync(Guid companyId)
        {
            return await _context.Warehouses
                .Where(w => w.CompanyId == companyId && w.IsActive)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }
    }
}