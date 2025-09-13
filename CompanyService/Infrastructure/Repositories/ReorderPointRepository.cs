using CompanyService.Core.Entities;
using CompanyService.Core.Interfaces;
using CompanyService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.Repositories
{
    public class ReorderPointRepository : Repository<ReorderPoint>, IReorderPointRepository
    {
        public ReorderPointRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ReorderPoint>> GetReorderPointsByCompanyAsync(Guid companyId)
        {
            return await _context.ReorderPoints
                .Include(rp => rp.Product)
                .Include(rp => rp.Warehouse)
                .Where(rp => rp.CompanyId == companyId)
                .OrderBy(rp => rp.Product.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReorderPoint>> GetReorderPointsByWarehouseAsync(Guid warehouseId, Guid companyId)
        {
            return await _context.ReorderPoints
                .Include(rp => rp.Product)
                .Include(rp => rp.Warehouse)
                .Where(rp => rp.WarehouseId == warehouseId && rp.CompanyId == companyId)
                .OrderBy(rp => rp.Product.Name)
                .ToListAsync();
        }

        public async Task<ReorderPoint?> GetReorderPointByIdAndCompanyAsync(Guid id, Guid companyId)
        {
            return await _context.ReorderPoints
                .Include(rp => rp.Product)
                .Include(rp => rp.Warehouse)
                .Include(rp => rp.ReorderAlerts)
                .FirstOrDefaultAsync(rp => rp.Id == id && rp.CompanyId == companyId);
        }

        public async Task<ReorderPoint?> GetReorderPointByProductAndWarehouseAsync(Guid productId, Guid warehouseId, Guid companyId)
        {
            return await _context.ReorderPoints
                .Include(rp => rp.Product)
                .Include(rp => rp.Warehouse)
                .Include(rp => rp.ReorderAlerts)
                .FirstOrDefaultAsync(rp => rp.ProductId == productId && 
                                          rp.WarehouseId == warehouseId && 
                                          rp.CompanyId == companyId);
        }

        public async Task<IEnumerable<ReorderPoint>> GetTriggeredReorderPointsAsync(Guid companyId)
        {
            return await _context.ReorderPoints
                .Include(rp => rp.Product)
                .Include(rp => rp.Warehouse)
                .Where(rp => rp.CompanyId == companyId && rp.Status == ReorderStatus.Triggered)
                .OrderBy(rp => rp.Product.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReorderPoint>> GetReorderPointsWithAlertsAsync(Guid companyId)
        {
            return await _context.ReorderPoints
                .Include(rp => rp.Product)
                .Include(rp => rp.Warehouse)
                .Include(rp => rp.ReorderAlerts.Where(a => !a.IsResolved))
                .Where(rp => rp.CompanyId == companyId && rp.ReorderAlerts.Any(a => !a.IsResolved))
                .OrderBy(rp => rp.Product.Name)
                .ToListAsync();
        }

        public async Task<bool> ExistsReorderPointForProductAndWarehouseAsync(Guid productId, Guid warehouseId, Guid companyId, Guid? excludeId = null)
        {
            var query = _context.ReorderPoints
                .Where(rp => rp.ProductId == productId && 
                            rp.WarehouseId == warehouseId && 
                            rp.CompanyId == companyId);

            if (excludeId.HasValue)
            {
                query = query.Where(rp => rp.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<ReorderAlert>> GetActiveReorderAlertsAsync(Guid companyId)
        {
            return await _context.Set<ReorderAlert>()
                .Include(ra => ra.ReorderPoint)
                    .ThenInclude(rp => rp.Product)
                .Include(ra => ra.ReorderPoint)
                    .ThenInclude(rp => rp.Warehouse)
                .Where(ra => ra.ReorderPoint.CompanyId == companyId && !ra.IsResolved)
                .OrderByDescending(ra => ra.AlertDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReorderAlert>> GetReorderAlertsByReorderPointAsync(Guid reorderPointId)
        {
            return await _context.Set<ReorderAlert>()
                .Where(ra => ra.ReorderPointId == reorderPointId)
                .OrderByDescending(ra => ra.AlertDate)
                .ToListAsync();
        }
    }
}