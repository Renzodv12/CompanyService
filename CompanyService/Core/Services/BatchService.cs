using CompanyService.Core.DTOs;
using CompanyService.Core.Entities;
using CompanyService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Core.Services
{
    public class BatchService : IBatchService
    {
        private readonly IRepository<Batch> _batchRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BatchService(
            IRepository<Batch> batchRepository,
            IRepository<Product> productRepository,
            IRepository<Warehouse> warehouseRepository,
            IUnitOfWork unitOfWork)
        {
            _batchRepository = batchRepository;
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BatchResponseDto>> GetBatchesByCompanyAsync(Guid companyId)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.CompanyId == companyId && b.IsActive);
            var batches = batchesQuery.ToList();

            var result = new List<BatchResponseDto>();
            foreach (var batch in batches)
            {
                var productsQuery = await _productRepository
                    .WhereAsync(p => p.Id == batch.ProductId);
                var product = productsQuery.FirstOrDefault();

                var warehousesQuery = await _warehouseRepository
                    .WhereAsync(w => w.Id == batch.WarehouseId);
                var warehouse = warehousesQuery.FirstOrDefault();

                result.Add(new BatchResponseDto
                {
                    Id = batch.Id,
                    BatchNumber = batch.BatchNumber,
                    ProductId = batch.ProductId,
                    ProductName = product?.Name ?? "Unknown",
                    ProductSku = product?.SKU ?? "Unknown",
                    WarehouseId = batch.WarehouseId,
                    WarehouseName = warehouse?.Name ?? "Unknown",
                    Quantity = batch.Quantity,
                    InitialQuantity = batch.InitialQuantity,
                    UnitCost = batch.UnitCost,
                    ManufactureDate = batch.ManufactureDate,
                    ExpirationDate = batch.ExpirationDate,
                    Supplier = batch.Supplier,
                    LotNumber = batch.LotNumber,
                    Notes = batch.Notes,
                    IsActive = batch.IsActive,
                    CreatedAt = batch.CreatedAt,
                    UpdatedAt = batch.UpdatedAt
                });
            }

            return result;
        }

        public async Task<IEnumerable<BatchResponseDto>> GetBatchesByWarehouseAsync(Guid warehouseId)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.WarehouseId == warehouseId && b.IsActive);
            var batches = batchesQuery.ToList();

            var result = new List<BatchResponseDto>();
            foreach (var batch in batches)
            {
                var productsQuery = await _productRepository
                    .WhereAsync(p => p.Id == batch.ProductId);
                var product = productsQuery.FirstOrDefault();

                var warehousesQuery = await _warehouseRepository
                    .WhereAsync(w => w.Id == batch.WarehouseId);
                var warehouse = warehousesQuery.FirstOrDefault();

                result.Add(new BatchResponseDto
                {
                    Id = batch.Id,
                    BatchNumber = batch.BatchNumber,
                    ProductId = batch.ProductId,
                    ProductName = product?.Name ?? "Unknown",
                    ProductSku = product?.SKU ?? "Unknown",
                    WarehouseId = batch.WarehouseId,
                    WarehouseName = warehouse?.Name ?? "Unknown",
                    Quantity = batch.Quantity,
                    InitialQuantity = batch.InitialQuantity,
                    UnitCost = batch.UnitCost,
                    ManufactureDate = batch.ManufactureDate,
                    ExpirationDate = batch.ExpirationDate,
                    Supplier = batch.Supplier,
                    LotNumber = batch.LotNumber,
                    Notes = batch.Notes,
                    IsActive = batch.IsActive,
                    CreatedAt = batch.CreatedAt,
                    UpdatedAt = batch.UpdatedAt
                });
            }

            return result;
        }

        public async Task<IEnumerable<BatchResponseDto>> GetBatchesByProductAsync(Guid productId)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.ProductId == productId && b.IsActive);
            var batches = batchesQuery.ToList();

            var result = new List<BatchResponseDto>();
            foreach (var batch in batches)
            {
                var productsQuery = await _productRepository
                    .WhereAsync(p => p.Id == batch.ProductId);
                var product = productsQuery.FirstOrDefault();

                var warehousesQuery = await _warehouseRepository
                    .WhereAsync(w => w.Id == batch.WarehouseId);
                var warehouse = warehousesQuery.FirstOrDefault();

                result.Add(new BatchResponseDto
                {
                    Id = batch.Id,
                    BatchNumber = batch.BatchNumber,
                    ProductId = batch.ProductId,
                    ProductName = product?.Name ?? "Unknown",
                    ProductSku = product?.SKU ?? "Unknown",
                    WarehouseId = batch.WarehouseId,
                    WarehouseName = warehouse?.Name ?? "Unknown",
                    Quantity = batch.Quantity,
                    InitialQuantity = batch.InitialQuantity,
                    UnitCost = batch.UnitCost,
                    ManufactureDate = batch.ManufactureDate,
                    ExpirationDate = batch.ExpirationDate,
                    Supplier = batch.Supplier,
                    LotNumber = batch.LotNumber,
                    Notes = batch.Notes,
                    IsActive = batch.IsActive,
                    CreatedAt = batch.CreatedAt,
                    UpdatedAt = batch.UpdatedAt
                });
            }

            return result;
        }

        public async Task<BatchResponseDto?> GetBatchByIdAsync(Guid id)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.Id == id);
            var batch = batchesQuery.FirstOrDefault();

            if (batch == null)
                return null;

            var productsQuery = await _productRepository
                .WhereAsync(p => p.Id == batch.ProductId);
            var product = productsQuery.FirstOrDefault();

            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == batch.WarehouseId);
            var warehouse = warehousesQuery.FirstOrDefault();

            return new BatchResponseDto
            {
                Id = batch.Id,
                BatchNumber = batch.BatchNumber,
                ProductId = batch.ProductId,
                ProductName = product?.Name ?? "Unknown",
                ProductSku = product?.SKU ?? "Unknown",
                WarehouseId = batch.WarehouseId,
                WarehouseName = warehouse?.Name ?? "Unknown",
                Quantity = batch.Quantity,
                InitialQuantity = batch.InitialQuantity,
                UnitCost = batch.UnitCost,
                ManufactureDate = batch.ManufactureDate,
                ExpirationDate = batch.ExpirationDate,
                Supplier = batch.Supplier,
                LotNumber = batch.LotNumber,
                Notes = batch.Notes,
                IsActive = batch.IsActive,
                CreatedAt = batch.CreatedAt,
                UpdatedAt = batch.UpdatedAt
            };
        }

        public async Task<BatchResponseDto> CreateBatchAsync(CreateBatchDto createBatchDto)
        {
            var batch = new Batch
            {
                Id = Guid.NewGuid(),
                BatchNumber = createBatchDto.BatchNumber,
                ProductId = createBatchDto.ProductId,
                WarehouseId = createBatchDto.WarehouseId,
                Quantity = createBatchDto.Quantity,
                InitialQuantity = createBatchDto.Quantity,
                UnitCost = createBatchDto.UnitCost,
                ManufactureDate = createBatchDto.ManufactureDate,
                ExpirationDate = createBatchDto.ExpirationDate,
                Supplier = createBatchDto.Supplier,
                LotNumber = createBatchDto.LotNumber,
                Notes = createBatchDto.Notes,
                CompanyId = createBatchDto.CompanyId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _batchRepository.AddAsync(batch);
            await _unitOfWork.SaveChangesAsync();

            var productsQuery = await _productRepository
                .WhereAsync(p => p.Id == batch.ProductId);
            var product = productsQuery.FirstOrDefault();

            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == batch.WarehouseId);
            var warehouse = warehousesQuery.FirstOrDefault();

            return new BatchResponseDto
            {
                Id = batch.Id,
                BatchNumber = batch.BatchNumber,
                ProductId = batch.ProductId,
                ProductName = product?.Name ?? "Unknown",
                ProductSku = product?.SKU ?? "Unknown",
                WarehouseId = batch.WarehouseId,
                WarehouseName = warehouse?.Name ?? "Unknown",
                Quantity = batch.Quantity,
                InitialQuantity = batch.InitialQuantity,
                UnitCost = batch.UnitCost,
                ManufactureDate = batch.ManufactureDate,
                ExpirationDate = batch.ExpirationDate,
                Supplier = batch.Supplier,
                LotNumber = batch.LotNumber,
                Notes = batch.Notes,
                IsActive = batch.IsActive,
                CreatedAt = batch.CreatedAt,
                UpdatedAt = batch.UpdatedAt
            };
        }

        public async Task<BatchResponseDto?> UpdateBatchAsync(Guid id, UpdateBatchDto updateBatchDto)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.Id == id);
            var batch = batchesQuery.FirstOrDefault();

            if (batch == null)
                return null;

            batch.BatchNumber = updateBatchDto.BatchNumber;
            batch.Quantity = updateBatchDto.Quantity;
            batch.UnitCost = updateBatchDto.UnitCost;
            batch.ManufactureDate = updateBatchDto.ManufactureDate;
            batch.ExpirationDate = updateBatchDto.ExpirationDate;
            batch.Supplier = updateBatchDto.Supplier;
            batch.LotNumber = updateBatchDto.LotNumber;
            batch.Notes = updateBatchDto.Notes;
            batch.IsActive = updateBatchDto.IsActive;
            batch.UpdatedAt = DateTime.UtcNow;

            _batchRepository.Update(batch);
            await _unitOfWork.SaveChangesAsync();

            var productsQuery = await _productRepository
                .WhereAsync(p => p.Id == batch.ProductId);
            var product = productsQuery.FirstOrDefault();

            var warehousesQuery = await _warehouseRepository
                .WhereAsync(w => w.Id == batch.WarehouseId);
            var warehouse = warehousesQuery.FirstOrDefault();

            return new BatchResponseDto
            {
                Id = batch.Id,
                BatchNumber = batch.BatchNumber,
                ProductId = batch.ProductId,
                ProductName = product?.Name ?? "Unknown",
                ProductSku = product?.SKU ?? "Unknown",
                WarehouseId = batch.WarehouseId,
                WarehouseName = warehouse?.Name ?? "Unknown",
                Quantity = batch.Quantity,
                InitialQuantity = batch.InitialQuantity,
                UnitCost = batch.UnitCost,
                ManufactureDate = batch.ManufactureDate,
                ExpirationDate = batch.ExpirationDate,
                Supplier = batch.Supplier,
                LotNumber = batch.LotNumber,
                Notes = batch.Notes,
                IsActive = batch.IsActive,
                CreatedAt = batch.CreatedAt,
                UpdatedAt = batch.UpdatedAt
            };
        }

        public async Task<bool> DeleteBatchAsync(Guid id)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.Id == id);
            var batch = batchesQuery.FirstOrDefault();

            if (batch == null)
                return false;

            _batchRepository.Remove(batch);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MoveBatchQuantityAsync(BatchMovementDto movementDto)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.Id == movementDto.BatchId);
            var batch = batchesQuery.FirstOrDefault();

            if (batch == null)
                return false;

            if (movementDto.MovementType.ToUpper() == "OUT")
            {
                if (batch.Quantity < movementDto.Quantity)
                    return false; // Insufficient quantity

                batch.Quantity -= movementDto.Quantity;
            }
            else if (movementDto.MovementType.ToUpper() == "IN")
            {
                batch.Quantity += movementDto.Quantity;
            }
            else
            {
                return false; // Invalid movement type
            }

            batch.UpdatedAt = DateTime.UtcNow;
            _batchRepository.Update(batch);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<BatchResponseDto>> GetExpiredBatchesAsync(Guid companyId)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.CompanyId == companyId && 
                               b.IsActive && 
                               b.ExpirationDate.HasValue && 
                               b.ExpirationDate < DateTime.UtcNow);
            var batches = batchesQuery.ToList();

            var result = new List<BatchResponseDto>();
            foreach (var batch in batches)
            {
                var productsQuery = await _productRepository
                    .WhereAsync(p => p.Id == batch.ProductId);
                var product = productsQuery.FirstOrDefault();

                var warehousesQuery = await _warehouseRepository
                    .WhereAsync(w => w.Id == batch.WarehouseId);
                var warehouse = warehousesQuery.FirstOrDefault();

                result.Add(new BatchResponseDto
                {
                    Id = batch.Id,
                    BatchNumber = batch.BatchNumber,
                    ProductId = batch.ProductId,
                    ProductName = product?.Name ?? "Unknown",
                    ProductSku = product?.SKU ?? "Unknown",
                    WarehouseId = batch.WarehouseId,
                    WarehouseName = warehouse?.Name ?? "Unknown",
                    Quantity = batch.Quantity,
                    InitialQuantity = batch.InitialQuantity,
                    UnitCost = batch.UnitCost,
                    ManufactureDate = batch.ManufactureDate,
                    ExpirationDate = batch.ExpirationDate,
                    Supplier = batch.Supplier,
                    LotNumber = batch.LotNumber,
                    Notes = batch.Notes,
                    IsActive = batch.IsActive,
                    CreatedAt = batch.CreatedAt,
                    UpdatedAt = batch.UpdatedAt
                });
            }

            return result;
        }

        public async Task<IEnumerable<BatchResponseDto>> GetNearExpirationBatchesAsync(Guid companyId, int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(days);
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.CompanyId == companyId && 
                               b.IsActive && 
                               b.ExpirationDate.HasValue && 
                               b.ExpirationDate >= DateTime.UtcNow && 
                               b.ExpirationDate <= cutoffDate);
            var batches = batchesQuery.ToList();

            var result = new List<BatchResponseDto>();
            foreach (var batch in batches)
            {
                var productsQuery = await _productRepository
                    .WhereAsync(p => p.Id == batch.ProductId);
                var product = productsQuery.FirstOrDefault();

                var warehousesQuery = await _warehouseRepository
                    .WhereAsync(w => w.Id == batch.WarehouseId);
                var warehouse = warehousesQuery.FirstOrDefault();

                result.Add(new BatchResponseDto
                {
                    Id = batch.Id,
                    BatchNumber = batch.BatchNumber,
                    ProductId = batch.ProductId,
                    ProductName = product?.Name ?? "Unknown",
                    ProductSku = product?.SKU ?? "Unknown",
                    WarehouseId = batch.WarehouseId,
                    WarehouseName = warehouse?.Name ?? "Unknown",
                    Quantity = batch.Quantity,
                    InitialQuantity = batch.InitialQuantity,
                    UnitCost = batch.UnitCost,
                    ManufactureDate = batch.ManufactureDate,
                    ExpirationDate = batch.ExpirationDate,
                    Supplier = batch.Supplier,
                    LotNumber = batch.LotNumber,
                    Notes = batch.Notes,
                    IsActive = batch.IsActive,
                    CreatedAt = batch.CreatedAt,
                    UpdatedAt = batch.UpdatedAt
                });
            }

            return result;
        }

        public async Task<IEnumerable<BatchResponseDto>> GetLowStockBatchesAsync(Guid companyId, decimal threshold = 10)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.CompanyId == companyId && 
                               b.IsActive && 
                               b.Quantity <= threshold);
            var batches = batchesQuery.ToList();

            var result = new List<BatchResponseDto>();
            foreach (var batch in batches)
            {
                var productsQuery = await _productRepository
                    .WhereAsync(p => p.Id == batch.ProductId);
                var product = productsQuery.FirstOrDefault();

                var warehousesQuery = await _warehouseRepository
                    .WhereAsync(w => w.Id == batch.WarehouseId);
                var warehouse = warehousesQuery.FirstOrDefault();

                result.Add(new BatchResponseDto
                {
                    Id = batch.Id,
                    BatchNumber = batch.BatchNumber,
                    ProductId = batch.ProductId,
                    ProductName = product?.Name ?? "Unknown",
                    ProductSku = product?.SKU ?? "Unknown",
                    WarehouseId = batch.WarehouseId,
                    WarehouseName = warehouse?.Name ?? "Unknown",
                    Quantity = batch.Quantity,
                    InitialQuantity = batch.InitialQuantity,
                    UnitCost = batch.UnitCost,
                    ManufactureDate = batch.ManufactureDate,
                    ExpirationDate = batch.ExpirationDate,
                    Supplier = batch.Supplier,
                    LotNumber = batch.LotNumber,
                    Notes = batch.Notes,
                    IsActive = batch.IsActive,
                    CreatedAt = batch.CreatedAt,
                    UpdatedAt = batch.UpdatedAt
                });
            }

            return result;
        }

        public async Task<object> GetBatchStatsAsync(Guid companyId)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.CompanyId == companyId && b.IsActive);
            var batches = batchesQuery.ToList();

            var totalValue = batches.Where(b => b.UnitCost.HasValue)
                                  .Sum(b => b.Quantity * b.UnitCost.Value);

            var expiredBatches = batches.Where(b => b.ExpirationDate.HasValue && 
                                              b.ExpirationDate < DateTime.UtcNow).Count();

            var nearExpirationBatches = batches.Where(b => b.ExpirationDate.HasValue && 
                                                     b.ExpirationDate >= DateTime.UtcNow &&
                                                     b.ExpirationDate <= DateTime.UtcNow.AddDays(30)).Count();

            var lowStockBatches = batches.Where(b => b.Quantity <= 10).Count();

            return new
            {
                TotalBatches = batches.Count,
                TotalProducts = batches.Select(b => b.ProductId).Distinct().Count(),
                TotalQuantity = batches.Sum(b => b.Quantity),
                TotalValue = totalValue,
                ExpiredBatches = expiredBatches,
                NearExpirationBatches = nearExpirationBatches,
                LowStockBatches = lowStockBatches,
                AverageQuantityPerBatch = batches.Count > 0 ? batches.Average(b => b.Quantity) : 0
            };
        }

        public async Task<decimal> GetAvailableQuantityAsync(Guid productId, Guid warehouseId)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.ProductId == productId && 
                               b.WarehouseId == warehouseId && 
                               b.IsActive);
            var batches = batchesQuery.ToList();

            return batches.Sum(b => b.Quantity);
        }

        public async Task<bool> BatchExistsAsync(Guid id)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.Id == id);
            return batchesQuery.Any();
        }

        public async Task<bool> BatchBelongsToCompanyAsync(Guid batchId, Guid companyId)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.Id == batchId && b.CompanyId == companyId);
            return batchesQuery.Any();
        }

        public async Task<bool> HasSufficientQuantityAsync(Guid batchId, decimal quantity)
        {
            var batchesQuery = await _batchRepository
                .WhereAsync(b => b.Id == batchId);
            var batch = batchesQuery.FirstOrDefault();

            return batch != null && batch.Quantity >= quantity;
        }
    }
}