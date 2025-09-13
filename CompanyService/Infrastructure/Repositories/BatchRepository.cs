using CompanyService.Core.Entities;
using CompanyService.Core.Interfaces;
using CompanyService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.Repositories
{
    public class BatchRepository : Repository<Batch>, IBatchRepository
    {
        public BatchRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Batch>> GetBatchesByCompanyAsync(Guid companyId)
        {
            return await _context.Batches
                .Include(b => b.Product)
                .Include(b => b.Warehouse)
                .Where(b => b.CompanyId == companyId)
                .OrderBy(b => b.ExpirationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Batch>> GetBatchesByProductAsync(Guid productId, Guid companyId)
        {
            return await _context.Batches
                .Include(b => b.Warehouse)
                .Where(b => b.ProductId == productId && b.CompanyId == companyId)
                .OrderBy(b => b.ExpirationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Batch>> GetBatchesByWarehouseAsync(Guid warehouseId, Guid companyId)
        {
            return await _context.Batches
                .Include(b => b.Product)
                .Where(b => b.WarehouseId == warehouseId && b.CompanyId == companyId)
                .OrderBy(b => b.ExpirationDate)
                .ToListAsync();
        }

        public async Task<Batch?> GetBatchByIdAndCompanyAsync(Guid id, Guid companyId)
        {
            return await _context.Batches
                .Include(b => b.Product)
                .Include(b => b.Warehouse)
                .FirstOrDefaultAsync(b => b.Id == id && b.CompanyId == companyId);
        }

        public async Task<IEnumerable<Batch>> GetExpiredBatchesAsync(Guid companyId, DateTime? asOfDate = null)
        {
            var checkDate = asOfDate ?? DateTime.UtcNow;
            return await _context.Batches
                .Include(b => b.Product)
                .Include(b => b.Warehouse)
                .Where(b => b.CompanyId == companyId && b.ExpirationDate < checkDate && b.Quantity > 0)
                .OrderBy(b => b.ExpirationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Batch>> GetBatchesExpiringInDaysAsync(Guid companyId, int days)
        {
            var checkDate = DateTime.UtcNow.AddDays(days);
            return await _context.Batches
                .Include(b => b.Product)
                .Include(b => b.Warehouse)
                .Where(b => b.CompanyId == companyId && 
                           b.ExpirationDate <= checkDate && 
                           b.ExpirationDate >= DateTime.UtcNow && 
                           b.Quantity > 0)
                .OrderBy(b => b.ExpirationDate)
                .ToListAsync();
        }

        public async Task<bool> ExistsBatchByNumberAndCompanyAsync(string batchNumber, Guid companyId, Guid? excludeId = null)
        {
            var query = _context.Batches
                .Where(b => b.BatchNumber.ToLower() == batchNumber.ToLower() && b.CompanyId == companyId);

            if (excludeId.HasValue)
            {
                query = query.Where(b => b.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<decimal> GetAvailableQuantityByProductAsync(Guid productId, Guid warehouseId, Guid companyId)
        {
            return await _context.Batches
                .Where(b => b.ProductId == productId && 
                           b.WarehouseId == warehouseId && 
                           b.CompanyId == companyId &&
                           b.Quantity > 0)
                .SumAsync(b => b.Quantity);
        }

        public async Task<IEnumerable<Batch>> GetBatchesWithLowStockAsync(Guid companyId, decimal minimumQuantity)
        {
            return await _context.Batches
                .Include(b => b.Product)
                .Include(b => b.Warehouse)
                .Where(b => b.CompanyId == companyId && b.Quantity <= minimumQuantity && b.Quantity > 0)
                .OrderBy(b => b.Quantity)
                .ToListAsync();
        }
    }
}