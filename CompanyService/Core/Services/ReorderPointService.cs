using CompanyService.Core.DTOs;
using CompanyService.Core.Entities;
using CompanyService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Core.Services
{
    public class ReorderPointService : IReorderPointService
    {
        private readonly IRepository<ReorderPoint> _reorderPointRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IRepository<Batch> _batchRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReorderPointService(
            IRepository<ReorderPoint> reorderPointRepository,
            IRepository<Product> productRepository,
            IRepository<Warehouse> warehouseRepository,
            IRepository<Batch> batchRepository,
            IUnitOfWork unitOfWork)
        {
            _reorderPointRepository = reorderPointRepository;
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _batchRepository = batchRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ReorderPointResponseDto>> GetReorderPointsByCompanyAsync(Guid companyId)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.CompanyId == companyId && rp.IsActive);
            var reorderPoints = reorderPointsQuery.ToList();

            var result = new List<ReorderPointResponseDto>();
            foreach (var reorderPoint in reorderPoints)
            {
                var productsQuery = await _productRepository
                    .WhereAsync(p => p.Id == reorderPoint.ProductId);
                var product = productsQuery.FirstOrDefault();

                var warehousesQuery = await _warehouseRepository
                    .WhereAsync(w => w.Id == reorderPoint.WarehouseId);
                var warehouse = warehousesQuery.FirstOrDefault();

                // Get current stock level
                var batchesQuery = await _batchRepository
                    .WhereAsync(b => b.ProductId == reorderPoint.ProductId && 
                                   b.WarehouseId == reorderPoint.WarehouseId && 
                                   b.IsActive);
                var currentStock = batchesQuery.Sum(b => b.Quantity);

                result.Add(new ReorderPointResponseDto
                {
                    Id = reorderPoint.Id,
                    ProductId = reorderPoint.ProductId,
                    ProductName = product?.Name ?? "Unknown",
                    ProductSku = product?.SKU ?? "Unknown",
                    WarehouseId = reorderPoint.WarehouseId,
                    WarehouseName = warehouse?.Name ?? "Unknown",
                    MinimumQuantity = reorderPoint.MinimumQuantity,
                    ReorderQuantity = reorderPoint.ReorderQuantity,
                    CurrentStock = currentStock,
                    Status = reorderPoint.Status,
                    LastTriggeredDate = reorderPoint.LastTriggeredDate,
                    IsActive = reorderPoint.IsActive,
                    CreatedAt = reorderPoint.CreatedAt,
                    UpdatedAt = reorderPoint.UpdatedAt
                });
            }

            return result;
        }

        public async Task<IEnumerable<ReorderPointResponseDto>> GetReorderPointsByWarehouseAsync(Guid warehouseId)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.WarehouseId == warehouseId && rp.IsActive);
            var reorderPoints = reorderPointsQuery.ToList();

            var result = new List<ReorderPointResponseDto>();
            foreach (var reorderPoint in reorderPoints)
            {
                var productsQuery = await _productRepository
                    .WhereAsync(p => p.Id == reorderPoint.ProductId);
                var product = productsQuery.FirstOrDefault();

                var warehousesQuery = await _warehouseRepository
                    .WhereAsync(w => w.Id == reorderPoint.WarehouseId);
                var warehouse = warehousesQuery.FirstOrDefault();

                // Get current stock level
                var batchesQuery = await _batchRepository
                    .WhereAsync(b => b.ProductId == reorderPoint.ProductId && 
                                   b.WarehouseId == reorderPoint.WarehouseId && 
                                   b.IsActive);
                var currentStock = batchesQuery.Sum(b => b.Quantity);

                result.Add(new ReorderPointResponseDto
                {
                    Id = reorderPoint.Id,
                    ProductId = reorderPoint.ProductId,
                    ProductName = product?.Name ?? "Unknown",
                    ProductSku = product?.SKU ?? "Unknown",
                    WarehouseId = reorderPoint.WarehouseId,
                    WarehouseName = warehouse?.Name ?? "Unknown",
                    MinimumQuantity = reorderPoint.MinimumQuantity,
                    ReorderQuantity = reorderPoint.ReorderQuantity,
                    CurrentStock = currentStock,
                    Status = reorderPoint.Status,
                    LastTriggeredDate = reorderPoint.LastTriggeredDate,
                    IsActive = reorderPoint.IsActive,
                    CreatedAt = reorderPoint.CreatedAt,
                    UpdatedAt = reorderPoint.UpdatedAt
                });
            }

            return result;
        }

        public async Task<ReorderPointResponseDto?> GetReorderPointByIdAsync(Guid id)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.Id == id);
            var reorderPoint = reorderPointsQuery.FirstOrDefault();

            if (reorderPoint == null)
                return null;

            var productsQuery = await _productRepository
                .WhereAsync(p => p.Id == reorderPoint.ProductId);
            var product = productsQuery.FirstOrDefault();

            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == reorderPoint.WarehouseId);
            var warehouse = warehousesQuery.FirstOrDefault();

            // Get current stock level
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.ProductId == reorderPoint.ProductId && 
                               b.WarehouseId == reorderPoint.WarehouseId && 
                               b.IsActive);
            var currentStock = batchesQuery.Sum(b => b.Quantity);

            return new ReorderPointResponseDto
            {
                Id = reorderPoint.Id,
                ProductId = reorderPoint.ProductId,
                ProductName = product?.Name ?? "Unknown",
                ProductSku = product?.SKU ?? "Unknown",
                WarehouseId = reorderPoint.WarehouseId,
                WarehouseName = warehouse?.Name ?? "Unknown",
                MinimumQuantity = reorderPoint.MinimumQuantity,
                ReorderQuantity = reorderPoint.ReorderQuantity,
                CurrentStock = currentStock,
                Status = reorderPoint.Status,
                LastTriggeredDate = reorderPoint.LastTriggeredDate,
                IsActive = reorderPoint.IsActive,
                CreatedAt = reorderPoint.CreatedAt,
                UpdatedAt = reorderPoint.UpdatedAt
            };
        }

        public async Task<ReorderPointResponseDto> CreateReorderPointAsync(CreateReorderPointDto createDto)
        {
            var reorderPoint = new ReorderPoint
            {
                Id = Guid.NewGuid(),
                ProductId = createDto.ProductId,
                WarehouseId = createDto.WarehouseId,
                MinimumQuantity = createDto.MinimumQuantity,
                ReorderQuantity = createDto.ReorderQuantity,
                Status = ReorderStatus.Active,
                CompanyId = createDto.CompanyId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _reorderPointRepository.AddAsync(reorderPoint);
            await _unitOfWork.SaveChangesAsync();

            var productsQuery = await _productRepository
                .WhereAsync(p => p.Id == reorderPoint.ProductId);
            var product = productsQuery.FirstOrDefault();

            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == reorderPoint.WarehouseId);
            var warehouse = warehousesQuery.FirstOrDefault();

            // Get current stock level
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.ProductId == reorderPoint.ProductId && 
                               b.WarehouseId == reorderPoint.WarehouseId && 
                               b.IsActive);
            var currentStock = batchesQuery.Sum(b => b.Quantity);

            return new ReorderPointResponseDto
            {
                Id = reorderPoint.Id,
                ProductId = reorderPoint.ProductId,
                ProductName = product?.Name ?? "Unknown",
                ProductSku = product?.SKU ?? "Unknown",
                WarehouseId = reorderPoint.WarehouseId,
                WarehouseName = warehouse?.Name ?? "Unknown",
                MinimumQuantity = reorderPoint.MinimumQuantity,
                ReorderQuantity = reorderPoint.ReorderQuantity,
                CurrentStock = currentStock,
                Status = reorderPoint.Status,
                LastTriggeredDate = reorderPoint.LastTriggeredDate,
                IsActive = reorderPoint.IsActive,
                CreatedAt = reorderPoint.CreatedAt,
                UpdatedAt = reorderPoint.UpdatedAt
            };
        }

        public async Task<ReorderPointResponseDto?> UpdateReorderPointAsync(Guid id, UpdateReorderPointDto updateDto)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.Id == id);
            var reorderPoint = reorderPointsQuery.FirstOrDefault();

            if (reorderPoint == null)
                return null;

            reorderPoint.MinimumQuantity = updateDto.MinimumQuantity;
            reorderPoint.ReorderQuantity = updateDto.ReorderQuantity;
            reorderPoint.IsActive = updateDto.IsActive;
            reorderPoint.UpdatedAt = DateTime.UtcNow;

            _reorderPointRepository.Update(reorderPoint);
            await _unitOfWork.SaveChangesAsync();

            var productsQuery = await _productRepository
                .WhereAsync(p => p.Id == reorderPoint.ProductId);
            var product = productsQuery.FirstOrDefault();

            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == reorderPoint.WarehouseId);
            var warehouse = warehousesQuery.FirstOrDefault();

            // Get current stock level
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.ProductId == reorderPoint.ProductId && 
                               b.WarehouseId == reorderPoint.WarehouseId && 
                               b.IsActive);
            var currentStock = batchesQuery.Sum(b => b.Quantity);

            return new ReorderPointResponseDto
            {
                Id = reorderPoint.Id,
                ProductId = reorderPoint.ProductId,
                ProductName = product?.Name ?? "Unknown",
                ProductSku = product?.SKU ?? "Unknown",
                WarehouseId = reorderPoint.WarehouseId,
                WarehouseName = warehouse?.Name ?? "Unknown",
                MinimumQuantity = reorderPoint.MinimumQuantity,
                ReorderQuantity = reorderPoint.ReorderQuantity,
                CurrentStock = currentStock,
                Status = reorderPoint.Status,
                LastTriggeredDate = reorderPoint.LastTriggeredDate,
                IsActive = reorderPoint.IsActive,
                CreatedAt = reorderPoint.CreatedAt,
                UpdatedAt = reorderPoint.UpdatedAt
            };
        }

        public async Task<bool> DeleteReorderPointAsync(Guid id)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.Id == id);
            var reorderPoint = reorderPointsQuery.FirstOrDefault();

            if (reorderPoint == null)
                return false;

            _reorderPointRepository.Remove(reorderPoint);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ReorderAlertDto>> GetActiveReorderAlertsAsync(Guid companyId)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.CompanyId == companyId && rp.IsActive);
            var reorderPoints = reorderPointsQuery.ToList();

            var alerts = new List<ReorderAlertDto>();

            foreach (var reorderPoint in reorderPoints)
            {
                // Get current stock level
                var batchesQuery = await _batchRepository
                    .WhereAsync(b => b.ProductId == reorderPoint.ProductId && 
                                   b.WarehouseId == reorderPoint.WarehouseId && 
                                   b.IsActive);
                var currentStock = batchesQuery.Sum(b => b.Quantity);

                if (currentStock <= reorderPoint.MinimumQuantity)
                {
                    var productsQuery = await _productRepository
                        .WhereAsync(p => p.Id == reorderPoint.ProductId);
                    var product = productsQuery.FirstOrDefault();

                    var warehousesQuery = await _warehouseRepository
                        .WhereAsync(w => w.Id == reorderPoint.WarehouseId);
                    var warehouse = warehousesQuery.FirstOrDefault();

                    alerts.Add(new ReorderAlertDto
                    {
                        Id = Guid.NewGuid(),
                        ReorderPointId = reorderPoint.Id,
                        ProductId = reorderPoint.ProductId,
                        ProductName = product?.Name ?? "Unknown",
                        ProductSku = product?.SKU ?? "Unknown",
                        WarehouseId = reorderPoint.WarehouseId,
                        WarehouseName = warehouse?.Name ?? "Unknown",
                        CurrentQuantity = currentStock,
                        TriggeredQuantity = reorderPoint.MinimumQuantity,
                        ReorderLevel = reorderPoint.ReorderLevel,
                        ReorderQuantity = reorderPoint.ReorderQuantity,
                        AlertDate = DateTime.UtcNow,
                        IsResolved = false
                    });
                }
            }

            return alerts.OrderBy(a => a.CurrentQuantity);
        }

        public async Task<bool> TriggerReorderPointAsync(Guid reorderPointId)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.Id == reorderPointId);
            var reorderPoint = reorderPointsQuery.FirstOrDefault();

            if (reorderPoint == null)
                return false;

            reorderPoint.Status = ReorderStatus.Triggered;
            reorderPoint.LastTriggeredDate = DateTime.UtcNow;
            reorderPoint.UpdatedAt = DateTime.UtcNow;

            _reorderPointRepository.Update(reorderPoint);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ResolveReorderPointAsync(Guid reorderPointId, ResolveReorderAlertDto resolveDto)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.Id == reorderPointId);
            var reorderPoint = reorderPointsQuery.FirstOrDefault();

            if (reorderPoint == null)
                return false;

            reorderPoint.Status = ReorderStatus.Active;
            reorderPoint.UpdatedAt = DateTime.UtcNow;

            _reorderPointRepository.Update(reorderPoint);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> CalculateOptimalReorderQuantityAsync(Guid productId, Guid warehouseId, int leadTimeDays = 7)
        {
            // Simple EOQ calculation based on historical consumption
            // This is a basic implementation - in practice, you'd want more sophisticated algorithms
            
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.ProductId == productId && 
                               b.WarehouseId == warehouseId && 
                               b.CreatedAt >= DateTime.UtcNow.AddDays(-90)); // Last 90 days
            var batches = batchesQuery.ToList();

            if (!batches.Any())
                return 100; // Default quantity

            // Calculate average daily consumption
            var totalConsumed = batches.Sum(b => b.InitialQuantity - b.Quantity);
            var averageDailyConsumption = totalConsumed / 90m;

            // Safety stock (lead time * average daily consumption * safety factor)
            var safetyStock = leadTimeDays * averageDailyConsumption * 1.5m;

            // Reorder quantity (lead time demand + safety stock)
            var reorderQuantity = (leadTimeDays * averageDailyConsumption) + safetyStock;

            return Math.Max(reorderQuantity, 10); // Minimum 10 units
        }

        public async Task<object> GetReorderPointStatsAsync(Guid companyId)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.CompanyId == companyId && rp.IsActive);
            var reorderPoints = reorderPointsQuery.ToList();

            var activeAlerts = 0;
            var criticalAlerts = 0;
            var highPriorityAlerts = 0;

            foreach (var reorderPoint in reorderPoints)
            {
                var batchesQuery = await _batchRepository
                    .WhereAsync(b => b.ProductId == reorderPoint.ProductId && 
                                   b.WarehouseId == reorderPoint.WarehouseId && 
                                   b.IsActive);
                var currentStock = batchesQuery.Sum(b => b.Quantity);

                if (currentStock <= reorderPoint.MinimumQuantity)
                {
                    activeAlerts++;
                    if (currentStock == 0)
                        criticalAlerts++;
                    else if (currentStock <= (reorderPoint.MinimumQuantity * 0.5m))
                        highPriorityAlerts++;
                }
            }

            return new
            {
                TotalReorderPoints = reorderPoints.Count,
                ActiveReorderPoints = reorderPoints.Count(rp => rp.IsActive),
                TriggeredReorderPoints = reorderPoints.Count(rp => rp.Status == ReorderStatus.Triggered),
                ActiveAlerts = activeAlerts,
                CriticalAlerts = criticalAlerts,
                HighPriorityAlerts = highPriorityAlerts,
                MediumPriorityAlerts = activeAlerts - criticalAlerts - highPriorityAlerts,
                AverageMinimumQuantity = reorderPoints.Count > 0 ? reorderPoints.Average(rp => rp.MinimumQuantity) : 0,
                AverageReorderQuantity = reorderPoints.Count > 0 ? reorderPoints.Average(rp => rp.ReorderQuantity) : 0
            };
        }

        public async Task<bool> CheckAndTriggerReorderPointsAsync(Guid companyId)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.CompanyId == companyId && rp.IsActive && rp.Status == ReorderStatus.Active);
            var reorderPoints = reorderPointsQuery.ToList();

            var triggeredCount = 0;

            foreach (var reorderPoint in reorderPoints)
            {
                var batchesQuery = await _batchRepository
                    .WhereAsync(b => b.ProductId == reorderPoint.ProductId && 
                                   b.WarehouseId == reorderPoint.WarehouseId && 
                                   b.IsActive);
                var currentStock = batchesQuery.Sum(b => b.Quantity);

                if (currentStock <= reorderPoint.MinimumQuantity)
                {
                    reorderPoint.Status = ReorderStatus.Triggered;
                    reorderPoint.LastTriggeredDate = DateTime.UtcNow;
                    reorderPoint.UpdatedAt = DateTime.UtcNow;
                    _reorderPointRepository.Update(reorderPoint);
                    triggeredCount++;
                }
            }

            if (triggeredCount > 0)
            {
                await _unitOfWork.SaveChangesAsync();
            }

            return triggeredCount > 0;
        }

        public async Task<bool> ReorderPointExistsAsync(Guid id)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.Id == id);
            return reorderPointsQuery.Any();
        }

        public async Task<bool> ReorderPointBelongsToCompanyAsync(Guid reorderPointId, Guid companyId)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.Id == reorderPointId && rp.CompanyId == companyId);
            return reorderPointsQuery.Any();
        }

        public async Task<bool> ReorderPointExistsForProductWarehouseAsync(Guid productId, Guid warehouseId)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.ProductId == productId && 
                                 rp.WarehouseId == warehouseId && 
                                 rp.IsActive);
            return reorderPointsQuery.Any();
        }

        public async Task<decimal> CalculateOptimalReorderQuantityAsync(Guid productId, Guid warehouseId)
        {
            // Simple calculation based on average consumption
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.ProductId == productId && b.WarehouseId == warehouseId);
            var batches = batchesQuery.ToList();
            
            if (!batches.Any())
                return 100; // Default quantity
            
            var averageQuantity = batches.Average(b => b.Quantity);
            return Math.Max(averageQuantity * 2, 50); // At least 50 units
        }

        public async Task<IEnumerable<ReorderPointResponseDto>> GetReorderPointsByStatusAsync(Guid companyId, ReorderStatus status)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.CompanyId == companyId && rp.Status == status);
            var reorderPoints = reorderPointsQuery.ToList();

            var result = new List<ReorderPointResponseDto>();
            foreach (var reorderPoint in reorderPoints)
            {
                var productsQuery = await _productRepository
                    .WhereAsync(p => p.Id == reorderPoint.ProductId);
                var product = productsQuery.FirstOrDefault();

                var warehousesQuery = await _warehouseRepository
                    .WhereAsync(w => w.Id == reorderPoint.WarehouseId);
                var warehouse = warehousesQuery.FirstOrDefault();

                result.Add(new ReorderPointResponseDto
                {
                    Id = reorderPoint.Id,
                    ProductId = reorderPoint.ProductId,
                    ProductName = product?.Name ?? "Unknown",
                    WarehouseId = reorderPoint.WarehouseId,
                    WarehouseName = warehouse?.Name ?? "Unknown",
                    MinimumQuantity = reorderPoint.MinimumQuantity,
                    ReorderQuantity = reorderPoint.ReorderQuantity,
                    Status = reorderPoint.Status,
                    IsActive = reorderPoint.IsActive,
                    LastTriggeredDate = reorderPoint.LastTriggeredDate,
                    CreatedAt = reorderPoint.CreatedAt,
                    UpdatedAt = reorderPoint.UpdatedAt
                });
            }

            return result;
        }

        public async Task<bool> ResolveReorderAlertAsync(Guid alertId, ResolveReorderAlertDto resolveDto)
        {
            // For now, just mark the reorder point as resolved
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.Id == alertId);
            var reorderPoint = reorderPointsQuery.FirstOrDefault();
            
            if (reorderPoint == null)
                return false;
            
            reorderPoint.Status = ReorderStatus.Active;
            reorderPoint.UpdatedAt = DateTime.UtcNow;
            _reorderPointRepository.Update(reorderPoint);
            await _unitOfWork.SaveChangesAsync();
            
            return true;
        }

        public async Task<bool> TriggerReorderPointAsync(Guid reorderPointId, decimal currentStock)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.Id == reorderPointId);
            var reorderPoint = reorderPointsQuery.FirstOrDefault();
            
            if (reorderPoint == null)
                return false;
            
            if (currentStock <= reorderPoint.MinimumQuantity)
            {
                reorderPoint.Status = ReorderStatus.Triggered;
                reorderPoint.LastTriggeredDate = DateTime.UtcNow;
                reorderPoint.UpdatedAt = DateTime.UtcNow;
                _reorderPointRepository.Update(reorderPoint);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            
            return false;
        }

        public async Task<bool> MarkAsOrderedAsync(Guid reorderPointId)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.Id == reorderPointId);
            var reorderPoint = reorderPointsQuery.FirstOrDefault();
            
            if (reorderPoint == null)
                return false;
            
            reorderPoint.Status = ReorderStatus.Ordered;
            reorderPoint.UpdatedAt = DateTime.UtcNow;
            _reorderPointRepository.Update(reorderPoint);
            await _unitOfWork.SaveChangesAsync();
            
            return true;
        }

        public async Task<object> GetReorderStatsAsync(Guid companyId)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.CompanyId == companyId && rp.IsActive);
            var reorderPoints = reorderPointsQuery.ToList();
            
            var totalReorderPoints = reorderPoints.Count;
            var triggeredCount = reorderPoints.Count(rp => rp.Status == ReorderStatus.Triggered);
            var orderedCount = reorderPoints.Count(rp => rp.Status == ReorderStatus.Ordered);
            var normalCount = reorderPoints.Count(rp => rp.Status == ReorderStatus.Active);
            
            return new
            {
                TotalReorderPoints = totalReorderPoints,
                TriggeredReorderPoints = triggeredCount,
                OrderedReorderPoints = orderedCount,
                ActiveReorderPoints = normalCount,
                ReorderPointsNeedingAttention = triggeredCount
            };
        }

        public async Task<IEnumerable<ReorderPointResponseDto>> GetReorderPointsByProductAsync(Guid productId)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.ProductId == productId && rp.IsActive);
            var reorderPoints = reorderPointsQuery.ToList();

            var result = new List<ReorderPointResponseDto>();
            foreach (var reorderPoint in reorderPoints)
            {
                var productsQuery = await _productRepository
                    .WhereAsync(p => p.Id == reorderPoint.ProductId);
                var product = productsQuery.FirstOrDefault();

                var warehousesQuery = await _warehouseRepository
                    .WhereAsync(w => w.Id == reorderPoint.WarehouseId);
                var warehouse = warehousesQuery.FirstOrDefault();

                result.Add(new ReorderPointResponseDto
                {
                    Id = reorderPoint.Id,
                    ProductId = reorderPoint.ProductId,
                    ProductName = product?.Name ?? "Unknown",
                    WarehouseId = reorderPoint.WarehouseId,
                    WarehouseName = warehouse?.Name ?? "Unknown",
                    MinimumQuantity = reorderPoint.MinimumQuantity,
                    ReorderQuantity = reorderPoint.ReorderQuantity,
                    Status = reorderPoint.Status,
                    IsActive = reorderPoint.IsActive,
                    LastTriggeredDate = reorderPoint.LastTriggeredDate,
                    CreatedAt = reorderPoint.CreatedAt,
                    UpdatedAt = reorderPoint.UpdatedAt
                });
            }

            return result;
        }

        public async Task<IEnumerable<ReorderPointResponseDto>> GetTriggeredReorderPointsAsync(Guid companyId)
        {
            return await GetReorderPointsByStatusAsync(companyId, ReorderStatus.Triggered);
        }

        public async Task<bool> ProductWarehouseCombinationExistsAsync(Guid productId, Guid warehouseId, Guid? excludeId = null)
        {
            var reorderPointsQuery = await _reorderPointRepository
                .WhereAsync(rp => rp.ProductId == productId && 
                                 rp.WarehouseId == warehouseId && 
                                 rp.IsActive &&
                                 (excludeId == null || rp.Id != excludeId));
            return reorderPointsQuery.Any();
        }
    }
}