using CompanyService.Core.Entities;
using CompanyService.Core.Interfaces;
using CompanyService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.Repositories
{
    public class PhysicalInventoryRepository : Repository<PhysicalInventory>, IPhysicalInventoryRepository
    {
        public PhysicalInventoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PhysicalInventory>> GetPhysicalInventoriesByCompanyAsync(Guid companyId)
        {
            return await _context.PhysicalInventories
                .Include(pi => pi.Warehouse)
                .Where(pi => pi.CompanyId == companyId)
                .OrderByDescending(pi => pi.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PhysicalInventory>> GetPhysicalInventoriesByWarehouseAsync(Guid warehouseId, Guid companyId)
        {
            return await _context.PhysicalInventories
                .Include(pi => pi.Warehouse)
                .Where(pi => pi.WarehouseId == warehouseId && pi.CompanyId == companyId)
                .OrderByDescending(pi => pi.ScheduledDate)
                .ToListAsync();
        }

        public async Task<PhysicalInventory?> GetPhysicalInventoryByIdAndCompanyAsync(Guid id, Guid companyId)
        {
            return await _context.PhysicalInventories
                .Include(pi => pi.Warehouse)
                .FirstOrDefaultAsync(pi => pi.Id == id && pi.CompanyId == companyId);
        }

        public async Task<PhysicalInventory?> GetPhysicalInventoryWithItemsAsync(Guid id, Guid companyId)
        {
            return await _context.PhysicalInventories
                .Include(pi => pi.Warehouse)
                .Include(pi => pi.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(pi => pi.Id == id && pi.CompanyId == companyId);
        }

        public async Task<IEnumerable<PhysicalInventory>> GetPhysicalInventoriesByStatusAsync(Guid companyId, PhysicalInventoryStatus status)
        {
            return await _context.PhysicalInventories
                .Include(pi => pi.Warehouse)
                .Where(pi => pi.CompanyId == companyId && pi.Status == status)
                .OrderByDescending(pi => pi.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PhysicalInventory>> GetPhysicalInventoriesByDateRangeAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            return await _context.PhysicalInventories
                .Include(pi => pi.Warehouse)
                .Where(pi => pi.CompanyId == companyId && 
                           pi.ScheduledDate >= startDate && 
                           pi.ScheduledDate <= endDate)
                .OrderByDescending(pi => pi.ScheduledDate)
                .ToListAsync();
        }

        public async Task<bool> ExistsActivePhysicalInventoryForWarehouseAsync(Guid warehouseId, Guid companyId)
        {
            return await _context.PhysicalInventories
                .AnyAsync(pi => pi.WarehouseId == warehouseId && 
                              pi.CompanyId == companyId && 
                              (pi.Status == PhysicalInventoryStatus.Planned || 
                               pi.Status == PhysicalInventoryStatus.InProgress));
        }

        public async Task<bool> ExistsPhysicalInventoryByNumberAndCompanyAsync(string inventoryNumber, Guid companyId, Guid? excludeId = null)
        {
            var query = _context.PhysicalInventories
                .Where(pi => pi.InventoryNumber.ToLower() == inventoryNumber.ToLower() && pi.CompanyId == companyId);

            if (excludeId.HasValue)
            {
                query = query.Where(pi => pi.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}