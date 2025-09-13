using CompanyService.Core.DTOs;
using CompanyService.Core.Entities;
using CompanyService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Core.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IRepository<Batch> _batchRepository;
        private readonly IRepository<PhysicalInventory> _physicalInventoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseService(
            IRepository<Warehouse> warehouseRepository,
            IRepository<Batch> batchRepository,
            IRepository<PhysicalInventory> physicalInventoryRepository,
            IUnitOfWork unitOfWork)
        {
            _warehouseRepository = warehouseRepository;
            _batchRepository = batchRepository;
            _physicalInventoryRepository = physicalInventoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WarehouseResponseDto>> GetWarehousesByCompanyAsync(Guid companyId)
        {
            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.CompanyId == companyId);
            var warehouses = warehousesQuery.ToList();

            var result = new List<WarehouseResponseDto>();
            foreach (var warehouse in warehouses)
            {
                var batchesQuery = await _batchRepository
                    .WhereAsync(b => b.WarehouseId == warehouse.Id && b.IsActive);
                var batches = batchesQuery.ToList();

                var inventoriesQuery = await _physicalInventoryRepository
                    .WhereAsync(pi => pi.WarehouseId == warehouse.Id);
                var inventories = inventoriesQuery.ToList();

                result.Add(new WarehouseResponseDto
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name,
                    Description = warehouse.Description,
                    Address = warehouse.Address,
                    City = warehouse.City,
                    State = warehouse.State,
                    PostalCode = warehouse.PostalCode,
                    Country = warehouse.Country,
                    Phone = warehouse.Phone,
                    Email = warehouse.Email,
                    IsActive = warehouse.IsActive,
                    CreatedAt = warehouse.CreatedAt,
                    UpdatedAt = warehouse.UpdatedAt,
                    TotalBatches = batches.Count,
                    ActiveInventories = inventories.Count(i => i.Status == PhysicalInventoryStatus.InProgress)
                });
            }

            return result;
        }

        public async Task<WarehouseResponseDto?> GetWarehouseByIdAsync(Guid id)
        {
            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == id);
            var warehouse = warehousesQuery.FirstOrDefault();

            if (warehouse == null)
                return null;

            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.WarehouseId == warehouse.Id && b.IsActive);
            var batches = batchesQuery.ToList();

            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.WarehouseId == warehouse.Id);
            var inventories = inventoriesQuery.ToList();

            return new WarehouseResponseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Description = warehouse.Description,
                Address = warehouse.Address,
                City = warehouse.City,
                State = warehouse.State,
                PostalCode = warehouse.PostalCode,
                Country = warehouse.Country,
                Phone = warehouse.Phone,
                Email = warehouse.Email,
                IsActive = warehouse.IsActive,
                CreatedAt = warehouse.CreatedAt,
                UpdatedAt = warehouse.UpdatedAt,
                TotalBatches = batches.Count,
                ActiveInventories = inventories.Count(i => i.Status == PhysicalInventoryStatus.InProgress)
            };
        }

        public async Task<WarehouseResponseDto> CreateWarehouseAsync(CreateWarehouseDto createWarehouseDto)
        {
            var warehouse = new Warehouse
            {
                Id = Guid.NewGuid(),
                Name = createWarehouseDto.Name,
                Description = createWarehouseDto.Description,
                Address = createWarehouseDto.Address,
                City = createWarehouseDto.City,
                State = createWarehouseDto.State,
                PostalCode = createWarehouseDto.PostalCode,
                Country = createWarehouseDto.Country,
                Phone = createWarehouseDto.Phone,
                Email = createWarehouseDto.Email,
                CompanyId = createWarehouseDto.CompanyId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _warehouseRepository.AddAsync(warehouse);
            await _unitOfWork.SaveChangesAsync();

            return new WarehouseResponseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Description = warehouse.Description,
                Address = warehouse.Address,
                City = warehouse.City,
                State = warehouse.State,
                PostalCode = warehouse.PostalCode,
                Country = warehouse.Country,
                Phone = warehouse.Phone,
                Email = warehouse.Email,
                IsActive = warehouse.IsActive,
                CreatedAt = warehouse.CreatedAt,
                UpdatedAt = warehouse.UpdatedAt,
                TotalBatches = 0,
                ActiveInventories = 0
            };
        }

        public async Task<WarehouseResponseDto?> UpdateWarehouseAsync(Guid id, UpdateWarehouseDto updateWarehouseDto)
        {
            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == id);
            var warehouse = warehousesQuery.FirstOrDefault();

            if (warehouse == null)
                return null;

            warehouse.Name = updateWarehouseDto.Name;
            warehouse.Description = updateWarehouseDto.Description;
            warehouse.Address = updateWarehouseDto.Address;
            warehouse.City = updateWarehouseDto.City;
            warehouse.State = updateWarehouseDto.State;
            warehouse.PostalCode = updateWarehouseDto.PostalCode;
            warehouse.Country = updateWarehouseDto.Country;
            warehouse.Phone = updateWarehouseDto.Phone;
            warehouse.Email = updateWarehouseDto.Email;
            warehouse.IsActive = updateWarehouseDto.IsActive;
            warehouse.UpdatedAt = DateTime.UtcNow;

            _warehouseRepository.Update(warehouse);
            await _unitOfWork.SaveChangesAsync();

            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.WarehouseId == warehouse.Id && b.IsActive);
            var batches = batchesQuery.ToList();

            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.WarehouseId == warehouse.Id);
            var inventories = inventoriesQuery.ToList();

            return new WarehouseResponseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Description = warehouse.Description,
                Address = warehouse.Address,
                City = warehouse.City,
                State = warehouse.State,
                PostalCode = warehouse.PostalCode,
                Country = warehouse.Country,
                Phone = warehouse.Phone,
                Email = warehouse.Email,
                IsActive = warehouse.IsActive,
                CreatedAt = warehouse.CreatedAt,
                UpdatedAt = warehouse.UpdatedAt,
                TotalBatches = batches.Count,
                ActiveInventories = inventories.Count(i => i.Status == PhysicalInventoryStatus.InProgress)
            };
        }

        public async Task<bool> DeleteWarehouseAsync(Guid id)
        {
            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == id);
            var warehouse = warehousesQuery.FirstOrDefault();

            if (warehouse == null)
                return false;

            // Check if warehouse has active batches
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.WarehouseId == id && b.IsActive);
            var activeBatches = batchesQuery.ToList();

            if (activeBatches.Any())
                return false; // Cannot delete warehouse with active batches

            _warehouseRepository.Remove(warehouse);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActivateWarehouseAsync(Guid id)
        {
            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == id);
            var warehouse = warehousesQuery.FirstOrDefault();

            if (warehouse == null)
                return false;

            warehouse.IsActive = true;
            warehouse.UpdatedAt = DateTime.UtcNow;

            _warehouseRepository.Update(warehouse);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateWarehouseAsync(Guid id)
        {
            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == id);
            var warehouse = warehousesQuery.FirstOrDefault();

            if (warehouse == null)
                return false;

            warehouse.IsActive = false;
            warehouse.UpdatedAt = DateTime.UtcNow;

            _warehouseRepository.Update(warehouse);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<WarehouseResponseDto>> GetActiveWarehousesByCompanyAsync(Guid companyId)
        {
            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.CompanyId == companyId && w.IsActive);
            var warehouses = warehousesQuery.ToList();

            var result = new List<WarehouseResponseDto>();
            foreach (var warehouse in warehouses)
            {
                var batchesQuery = await _batchRepository
                    .WhereAsync(b => b.WarehouseId == warehouse.Id && b.IsActive);
                var batches = batchesQuery.ToList();

                var inventoriesQuery = await _physicalInventoryRepository
                    .WhereAsync(pi => pi.WarehouseId == warehouse.Id);
                var inventories = inventoriesQuery.ToList();

                result.Add(new WarehouseResponseDto
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name,
                    Description = warehouse.Description,
                    Address = warehouse.Address,
                    City = warehouse.City,
                    State = warehouse.State,
                    PostalCode = warehouse.PostalCode,
                    Country = warehouse.Country,
                    Phone = warehouse.Phone,
                    Email = warehouse.Email,
                    IsActive = warehouse.IsActive,
                    CreatedAt = warehouse.CreatedAt,
                    UpdatedAt = warehouse.UpdatedAt,
                    TotalBatches = batches.Count,
                    ActiveInventories = inventories.Count(i => i.Status == PhysicalInventoryStatus.InProgress)
                });
            }

            return result;
        }

        public async Task<object> GetWarehouseStatsAsync(Guid warehouseId)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.WarehouseId == warehouseId && b.IsActive);
            var batches = batchesQuery.ToList();

            var inventoriesQuery = await _physicalInventoryRepository
                .WhereAsync(pi => pi.WarehouseId == warehouseId);
            var inventories = inventoriesQuery.ToList();

            var totalValue = batches.Where(b => b.UnitCost.HasValue)
                                  .Sum(b => b.Quantity * b.UnitCost.Value);

            var expiredBatches = batches.Where(b => b.ExpirationDate.HasValue && 
                                              b.ExpirationDate < DateTime.UtcNow).Count();

            var nearExpirationBatches = batches.Where(b => b.ExpirationDate.HasValue && 
                                                     b.ExpirationDate >= DateTime.UtcNow &&
                                                     b.ExpirationDate <= DateTime.UtcNow.AddDays(30)).Count();

            return new
            {
                TotalBatches = batches.Count,
                TotalProducts = batches.Select(b => b.ProductId).Distinct().Count(),
                TotalQuantity = batches.Sum(b => b.Quantity),
                TotalValue = totalValue,
                ExpiredBatches = expiredBatches,
                NearExpirationBatches = nearExpirationBatches,
                TotalInventories = inventories.Count,
                CompletedInventories = inventories.Count(i => i.Status == PhysicalInventoryStatus.Completed),
                InProgressInventories = inventories.Count(i => i.Status == PhysicalInventoryStatus.InProgress)
            };
        }

        public async Task<bool> WarehouseExistsAsync(Guid id)
        {
            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == id);
            return warehousesQuery.Any();
        }

        public async Task<bool> WarehouseBelongsToCompanyAsync(Guid warehouseId, Guid companyId)
        {
            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == warehouseId && w.CompanyId == companyId);
            return warehousesQuery.Any();
        }
    }
}