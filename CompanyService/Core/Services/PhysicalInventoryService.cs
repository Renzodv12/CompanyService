using CompanyService.Core.DTOs;
using CompanyService.Core.Entities;
using CompanyService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace CompanyService.Core.Services
{
    public class PhysicalInventoryService : IPhysicalInventoryService
    {
        private readonly IRepository<PhysicalInventory> _physicalInventoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IRepository<Batch> _batchRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PhysicalInventoryService(
            IRepository<PhysicalInventory> physicalInventoryRepository,
            IRepository<Product> productRepository,
            IRepository<Warehouse> warehouseRepository,
            IRepository<Batch> batchRepository,
            IUnitOfWork unitOfWork)
        {
            _physicalInventoryRepository = physicalInventoryRepository;
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _batchRepository = batchRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PhysicalInventoryResponseDto>> GetPhysicalInventoriesByCompanyAsync(Guid companyId)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.CompanyId == companyId);
            var inventories = inventoriesQuery.ToList();

            var result = new List<PhysicalInventoryResponseDto>();
            foreach (var inventory in inventories)
            {
                var warehousesQuery = await _warehouseRepository
                    .WhereAsync(w => w.Id == inventory.WarehouseId);
                var warehouse = warehousesQuery.FirstOrDefault();

                result.Add(new PhysicalInventoryResponseDto
                {
                    Id = inventory.Id,
                    InventoryNumber = inventory.InventoryNumber,
                    WarehouseId = inventory.WarehouseId,
                    WarehouseName = warehouse?.Name ?? "Unknown",
                    ScheduledDate = inventory.ScheduledDate,
                    StartDate = inventory.StartDate,
                    CompletedDate = inventory.CompletedDate,
                    Status = inventory.Status,
                    Notes = inventory.Notes,
                TotalItems = inventory.Items?.Count ?? 0,
                CreatedAt = inventory.CreatedAt,
                    UpdatedAt = inventory.UpdatedAt
                });
            }

            return result;
        }

        public async Task<IEnumerable<PhysicalInventoryResponseDto>> GetPhysicalInventoriesByWarehouseAsync(Guid warehouseId)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.WarehouseId == warehouseId);
            var inventories = inventoriesQuery.ToList();

            var result = new List<PhysicalInventoryResponseDto>();
            foreach (var inventory in inventories)
            {
                var warehousesQuery = await _warehouseRepository
                    .WhereAsync(w => w.Id == inventory.WarehouseId);
                var warehouse = warehousesQuery.FirstOrDefault();

                result.Add(new PhysicalInventoryResponseDto
                {
                    Id = inventory.Id,
                    InventoryNumber = inventory.InventoryNumber,
                    WarehouseId = inventory.WarehouseId,
                    WarehouseName = warehouse?.Name ?? "Unknown",
                    ScheduledDate = inventory.ScheduledDate,
                    StartDate = inventory.StartDate,
                    CompletedDate = inventory.CompletedDate,
                    Status = inventory.Status,
                    Notes = inventory.Notes,
                TotalItems = inventory.Items?.Count ?? 0,
                CreatedAt = inventory.CreatedAt,
                    UpdatedAt = inventory.UpdatedAt
                });
            }

            return result;
        }

        public async Task<PhysicalInventoryResponseDto?> GetPhysicalInventoryByIdAsync(Guid id)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == id);
            var inventory = inventoriesQuery.FirstOrDefault();

            if (inventory == null)
                return null;

            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == inventory.WarehouseId);
            var warehouse = warehousesQuery.FirstOrDefault();

            var items = new List<PhysicalInventoryItemDto>();
            if (inventory.Items != null)
            {
                foreach (var item in inventory.Items)
                {
                    var productsQuery = await _productRepository
                        .WhereAsync(p => p.Id == item.ProductId);
                    var product = productsQuery.FirstOrDefault();

                    items.Add(new PhysicalInventoryItemDto
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        ProductName = product?.Name ?? "Unknown",
                        SystemQuantity = item.SystemQuantity,
                        CountedQuantity = item.CountedQuantity,
                        Variance = item.CountedQuantity.HasValue ? item.CountedQuantity.Value - item.SystemQuantity : null,
                        Notes = item.Notes,
                        CountedBy = item.CountedBy,
                        CountedAt = item.CountedAt
                    });
                }
            }

            return new PhysicalInventoryResponseDto
            {
                Id = inventory.Id,
                WarehouseId = inventory.WarehouseId,
                Description = inventory.Description,
                Status = inventory.Status,
                ScheduledDate = inventory.ScheduledDate,
                StartDate = inventory.StartDate,
                CompletedDate = inventory.CompletedDate,
                CreatedAt = inventory.CreatedAt,
                UpdatedAt = inventory.UpdatedAt
            };
        }

        public async Task<PhysicalInventoryResponseDto> CreatePhysicalInventoryAsync(CreatePhysicalInventoryDto createDto)
        {
            var inventory = new PhysicalInventory
            {
                Id = Guid.NewGuid(),
                InventoryNumber = await GenerateInventoryNumberAsync(),
                WarehouseId = createDto.WarehouseId,
                ScheduledDate = createDto.ScheduledDate,
                Status = PhysicalInventoryStatus.Planned,
                Notes = createDto.Notes,
                CompanyId = createDto.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            await _physicalInventoryRepository.AddAsync(inventory);
            await _unitOfWork.SaveChangesAsync();

            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == inventory.WarehouseId);
            var warehouse = warehousesQuery.FirstOrDefault();

            return new PhysicalInventoryResponseDto
            {
                Id = inventory.Id,
                InventoryNumber = inventory.InventoryNumber,
                WarehouseId = inventory.WarehouseId,
                WarehouseName = warehouse?.Name ?? "Unknown",
                ScheduledDate = inventory.ScheduledDate,
                StartDate = inventory.StartDate,
                CompletedDate = inventory.CompletedDate,
                Status = inventory.Status,
                Notes = inventory.Notes,
                TotalItems = 0,
                CreatedAt = inventory.CreatedAt,
                UpdatedAt = inventory.UpdatedAt
            };
        }

        public async Task<PhysicalInventoryResponseDto?> UpdatePhysicalInventoryAsync(Guid id, UpdatePhysicalInventoryDto updateDto)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == id);
            var inventory = inventoriesQuery.FirstOrDefault();

            if (inventory == null)
                return null;

            inventory.ScheduledDate = updateDto.ScheduledDate;
            inventory.Notes = updateDto.Notes;
            inventory.UpdatedAt = DateTime.UtcNow;

            _physicalInventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();

            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == inventory.WarehouseId);
            var warehouse = warehousesQuery.FirstOrDefault();

            return new PhysicalInventoryResponseDto
            {
                Id = inventory.Id,
                InventoryNumber = inventory.InventoryNumber,
                WarehouseId = inventory.WarehouseId,
                WarehouseName = warehouse?.Name ?? "Unknown",
                ScheduledDate = inventory.ScheduledDate,
                StartDate = inventory.StartDate,
                CompletedDate = inventory.CompletedDate,
                Status = inventory.Status,
                Notes = inventory.Notes,
                TotalItems = inventory.Items?.Count ?? 0,
                CreatedAt = inventory.CreatedAt,
                UpdatedAt = inventory.UpdatedAt
            };
        }

        public async Task<bool> DeletePhysicalInventoryAsync(Guid id)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == id);
            var inventory = inventoriesQuery.FirstOrDefault();

            if (inventory == null)
                return false;

            _physicalInventoryRepository.Remove(inventory);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> StartPhysicalInventoryAsync(Guid id, string startedBy)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == id);
            var inventory = inventoriesQuery.FirstOrDefault();

            if (inventory == null || inventory.Status != PhysicalInventoryStatus.Planned)
                return false;

            // Generate inventory items based on current stock
            await GenerateInventoryItemsAsync(inventory);

            inventory.Status = PhysicalInventoryStatus.InProgress;
            inventory.StartDate = DateTime.UtcNow;
            inventory.UpdatedAt = DateTime.UtcNow;

            _physicalInventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CompletePhysicalInventoryAsync(Guid id, string completedBy)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == id);
            var inventory = inventoriesQuery.FirstOrDefault();

            if (inventory == null || inventory.Status != PhysicalInventoryStatus.InProgress)
                return false;

            // Check if all items have been counted
            if (inventory.Items?.Any(i => !i.CountedQuantity.HasValue) == true)
                return false; // Not all items counted

            inventory.Status = PhysicalInventoryStatus.Completed;
            inventory.CompletedDate = DateTime.UtcNow;
            inventory.UpdatedAt = DateTime.UtcNow;

            _physicalInventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateInventoryItemCountAsync(Guid inventoryId, UpdatePhysicalInventoryItemDto updateDto)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == inventoryId);
            var inventory = inventoriesQuery.FirstOrDefault();

            if (inventory == null || inventory.Status != PhysicalInventoryStatus.InProgress)
                return false;

            // Este método necesita ser revisado ya que UpdatePhysicalInventoryItemDto no tiene ItemId
            // Por ahora, comentamos esta implementación
            return false;
            
            /*
            var item = inventory.Items?.FirstOrDefault(i => i.Id == updateDto.ItemId);
            if (item == null)
                return false;

            item.CountedQuantity = updateDto.CountedQuantity;
            item.Notes = updateDto.Notes;
            item.CountedBy = updateDto.CountedBy;
            item.CountedAt = DateTime.UtcNow;

            inventory.UpdatedAt = DateTime.UtcNow;

            _physicalInventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();

            return true;
            */
        }

        public async Task<object> GetInventoryVarianceReportAsync(Guid inventoryId)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == inventoryId);
            var inventory = inventoriesQuery.FirstOrDefault();

            if (inventory == null)
                return null;

            var variances = new List<object>();
            decimal totalVarianceValue = 0;

            if (inventory.Items != null)
            {
                foreach (var item in inventory.Items.Where(i => i.CountedQuantity.HasValue))
                {
                    var variance = item.CountedQuantity.Value - item.SystemQuantity;
                    if (variance != 0)
                    {
                        var productsQuery = await _productRepository
                            .WhereAsync(p => p.Id == item.ProductId);
                        var product = productsQuery.FirstOrDefault();

                        var varianceValue = variance * 0; // UnitCost not available
                        totalVarianceValue += varianceValue;

                        variances.Add(new
                        {
                            ProductId = item.ProductId,
                            ProductName = product?.Name ?? "Unknown",
                            ProductSku = product?.SKU ?? "Unknown",
                            SystemQuantity = item.SystemQuantity,
                            CountedQuantity = item.CountedQuantity.Value,
                            Variance = variance,
                            // UnitCost not available in PhysicalInventoryItem
                            VarianceValue = varianceValue,
                            VariancePercentage = item.SystemQuantity > 0 ? (variance / item.SystemQuantity) * 100 : 0
                        });
                    }
                }
            }

            return new
            {
                InventoryId = inventoryId,
                InventoryNumber = inventory.InventoryNumber,
                TotalItems = inventory.Items?.Count ?? 0,
                ItemsWithVariance = variances.Count,
                TotalVarianceValue = totalVarianceValue,
                Variances = variances
            };
        }

        public async Task<object> GetInventoryStatsAsync(Guid companyId)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.CompanyId == companyId);
            var inventories = inventoriesQuery.ToList();

            var scheduledCount = inventories.Count(i => i.Status == PhysicalInventoryStatus.Planned);
            var inProgressCount = inventories.Count(i => i.Status == PhysicalInventoryStatus.InProgress);
            var completedCount = inventories.Count(i => i.Status == PhysicalInventoryStatus.Completed);

            var completedInventories = inventories.Where(i => i.Status == PhysicalInventoryStatus.Completed).ToList();
            var totalVarianceValue = 0m;
            var totalItemsWithVariance = 0;

            foreach (var inventory in completedInventories)
            {
                if (inventory.Items != null)
                {
                    foreach (var item in inventory.Items.Where(i => i.CountedQuantity.HasValue))
                    {
                        var variance = item.CountedQuantity.Value - item.SystemQuantity;
                        if (variance != 0)
                        {
                            totalItemsWithVariance++;
                            totalVarianceValue += variance * 0; // UnitCost not available
                        }
                    }
                }
            }

            return new
            {
                TotalInventories = inventories.Count,
                ScheduledInventories = scheduledCount,
                InProgressInventories = inProgressCount,
                CompletedInventories = completedCount,
                TotalItemsWithVariance = totalItemsWithVariance,
                TotalVarianceValue = totalVarianceValue,
                AverageCompletionTime = completedInventories.Where(i => i.StartDate.HasValue && i.CompletedDate.HasValue)
                                                          .Select(i => (i.CompletedDate.Value - i.StartDate.Value).TotalHours)
                                                          .DefaultIfEmpty(0)
                                                          .Average()
            };
        }

        private async Task<string> GenerateInventoryNumberAsync()
        {
            var today = DateTime.UtcNow;
            var prefix = $"INV-{today:yyyyMM}";
            
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.InventoryNumber.StartsWith(prefix));
            var count = inventoriesQuery.Count();
            
            return $"{prefix}-{(count + 1):D4}";
        }

        private async Task GenerateInventoryItemsAsync(PhysicalInventory inventory)
        {
            // Get all active batches for the warehouse
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.WarehouseId == inventory.WarehouseId && b.IsActive);
            var batches = batchesQuery.ToList();

            // Group by product and sum quantities
            var productQuantities = batches.GroupBy(b => b.ProductId)
                                          .Select(g => new
                                          {
                                              ProductId = g.Key,
                                              SystemQuantity = g.Sum(b => b.Quantity)
                                          })
                                          .ToList();

            inventory.Items = new List<PhysicalInventoryItem>();
            foreach (var pq in productQuantities)
            {
                inventory.Items.Add(new PhysicalInventoryItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = pq.ProductId,
                    SystemQuantity = pq.SystemQuantity
                });
            }
        }

        public async Task<bool> PhysicalInventoryExistsAsync(Guid id)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == id);
            return inventoriesQuery.Any();
        }

        public async Task<bool> PhysicalInventoryBelongsToCompanyAsync(Guid inventoryId, Guid companyId)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == inventoryId && pi.CompanyId == companyId);
            return inventoriesQuery.Any();
        }

        public async Task<bool> CanModifyPhysicalInventoryAsync(Guid id)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == id);
            var inventory = inventoriesQuery.FirstOrDefault();

            return inventory != null && inventory.Status != PhysicalInventoryStatus.Completed;
        }

        public async Task<IEnumerable<PhysicalInventoryItemResponseDto>> GetInventoryItemsAsync(Guid inventoryId)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == inventoryId);
            var inventory = inventoriesQuery.FirstOrDefault();
            
            if (inventory?.Items == null)
                return new List<PhysicalInventoryItemResponseDto>();
            
            var result = new List<PhysicalInventoryItemResponseDto>();
            foreach (var item in inventory.Items)
            {
                var productsQuery = await _productRepository
                    .WhereAsync(p => p.Id == item.ProductId);
                var product = productsQuery.FirstOrDefault();
                
                result.Add(new PhysicalInventoryItemResponseDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = product?.Name ?? "Unknown",
                    SystemQuantity = item.SystemQuantity,
                    CountedQuantity = item.CountedQuantity,
                    Notes = item.Notes
                });
            }
            
            return result;
        }

        public async Task<PhysicalInventoryItemResponseDto?> UpdateInventoryItemAsync(Guid inventoryId, Guid itemId, UpdatePhysicalInventoryItemDto updateDto)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == inventoryId);
            var inventory = inventoriesQuery.FirstOrDefault();
            
            if (inventory?.Items == null)
                return null;
            
            var item = inventory.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return null;
            
            item.CountedQuantity = updateDto.CountedQuantity;
            item.Notes = updateDto.Notes;
            
            _physicalInventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();
            
            var productsQuery = await _productRepository
                .WhereAsync(p => p.Id == item.ProductId);
            var product = productsQuery.FirstOrDefault();
            
            return new PhysicalInventoryItemResponseDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = product?.Name ?? "Unknown",
                SystemQuantity = item.SystemQuantity,
                CountedQuantity = item.CountedQuantity,
                Notes = item.Notes
            };
        }

        public async Task<bool> StartPhysicalInventoryAsync(Guid id)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == id);
            var inventory = inventoriesQuery.FirstOrDefault();
            
            if (inventory == null || inventory.Status != PhysicalInventoryStatus.Planned)
                return false;
            
            inventory.Status = PhysicalInventoryStatus.InProgress;
            inventory.StartDate = DateTime.UtcNow;
            inventory.UpdatedAt = DateTime.UtcNow;
            
            _physicalInventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();
            
            return true;
        }

        public async Task<bool> CompletePhysicalInventoryAsync(Guid id)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == id);
            var inventory = inventoriesQuery.FirstOrDefault();
            
            if (inventory == null || inventory.Status != PhysicalInventoryStatus.InProgress)
                return false;
            
            inventory.Status = PhysicalInventoryStatus.Completed;
            inventory.CompletedDate = DateTime.UtcNow;
            inventory.UpdatedAt = DateTime.UtcNow;
            
            _physicalInventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();
            
            return true;
        }

        public async Task<bool> CancelPhysicalInventoryAsync(Guid id)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.Id == id);
            var inventory = inventoriesQuery.FirstOrDefault();
            
            if (inventory == null || inventory.Status == PhysicalInventoryStatus.Completed)
                return false;
            
            inventory.Status = PhysicalInventoryStatus.Cancelled;
            inventory.UpdatedAt = DateTime.UtcNow;
            
            _physicalInventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();
            
            return true;
        }

        public async Task<IEnumerable<PhysicalInventoryResponseDto>> GetInventoriesByStatusAsync(Guid companyId, PhysicalInventoryStatus status)
        {
            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.CompanyId == companyId && pi.Status == status);
            var inventories = inventoriesQuery.ToList();

            var result = new List<PhysicalInventoryResponseDto>();
            foreach (var inventory in inventories)
            {
                var warehousesQuery = await _warehouseRepository
                    .WhereAsync(w => w.Id == inventory.WarehouseId);
                var warehouse = warehousesQuery.FirstOrDefault();

                result.Add(new PhysicalInventoryResponseDto
                {
                    Id = inventory.Id,
                    InventoryNumber = inventory.InventoryNumber,
                    WarehouseId = inventory.WarehouseId,
                    WarehouseName = warehouse?.Name ?? "Unknown",
                    ScheduledDate = inventory.ScheduledDate,
                    StartDate = inventory.StartDate,
                    CompletedDate = inventory.CompletedDate,
                    Status = inventory.Status,
                    Notes = inventory.Notes,
                    CreatedAt = inventory.CreatedAt,
                    UpdatedAt = inventory.UpdatedAt
                });
            }

            return result;
        }

        public async Task<bool> CanModifyInventoryAsync(Guid inventoryId)
        {
            return await CanModifyPhysicalInventoryAsync(inventoryId);
        }
    }
}