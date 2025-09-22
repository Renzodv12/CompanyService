using CompanyService.Core.DTOs;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CompanyService.WebApi.Endpoints
{
    public static class BatchEndpoints
    {
        public static void MapBatchEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/batches")
                .WithTags("Batches")
                .RequireAuthorization();

            // Basic CRUD operations
            group.MapGet("/", GetBatches)
                .WithName("GetBatches")
                .WithOpenApi();

            group.MapGet("/{id:guid}", GetBatch)
                .WithName("GetBatch")
                .WithOpenApi();

            group.MapPost("/", CreateBatch)
                .WithName("CreateBatch")
                .WithOpenApi();

            group.MapPut("/{id:guid}", UpdateBatch)
                .WithName("UpdateBatch")
                .WithOpenApi();

            // Query operations
            group.MapGet("/product/{productId:guid}", GetBatchesByProduct)
                .WithName("GetBatchesByProduct")
                .WithOpenApi();

            group.MapGet("/warehouse/{warehouseId:guid}", GetBatchesByWarehouse)
                .WithName("GetBatchesByWarehouse")
                .WithOpenApi();

            group.MapGet("/expired", GetExpiredBatches)
                .WithName("GetExpiredBatches")
                .WithOpenApi();

            group.MapGet("/near-expiration", GetNearExpirationBatches)
                .WithName("GetNearExpirationBatches")
                .WithOpenApi();

            group.MapGet("/availability/{productId:guid}/{warehouseId:guid}", GetAvailableQuantity)
                .WithName("GetAvailableQuantity")
                .WithOpenApi();

            // Operations
            group.MapPost("/move", MoveBatchQuantity)
                .WithName("MoveBatchQuantity")
                .WithOpenApi();
        }

        private static async Task<IResult> GetBatches(
            ClaimsPrincipal user,
            IBatchService batchService,
            ILogger<IBatchService> logger)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("ID de empresa no válido");

                var batches = await batchService.GetBatchesByCompanyAsync(companyId);
                return Results.Ok(batches);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener lotes");
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> GetBatch(
            Guid id,
            ClaimsPrincipal user,
            IBatchService batchService,
            ILogger<IBatchService> logger)
        {
            try
            {
                var batch = await batchService.GetBatchByIdAsync(id);
                if (batch == null)
                    return Results.NotFound("Lote no encontrado");

                return Results.Ok(batch);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener lote {Id}", id);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> GetBatchesByProduct(
            Guid productId,
            ClaimsPrincipal user,
            IBatchService batchService,
            ILogger<IBatchService> logger)
        {
            try
            {
                var batches = await batchService.GetBatchesByProductAsync(productId);
                return Results.Ok(batches);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener lotes del producto {ProductId}", productId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> GetBatchesByWarehouse(
            Guid warehouseId,
            ClaimsPrincipal user,
            IBatchService batchService,
            ILogger<IBatchService> logger)
        {
            try
            {
                var batches = await batchService.GetBatchesByWarehouseAsync(warehouseId);
                return Results.Ok(batches);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener lotes del almacén {WarehouseId}", warehouseId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> CreateBatch(
            CreateBatchDto createBatchDto,
            ClaimsPrincipal user,
            IBatchService batchService,
            ILogger<IBatchService> logger)
        {
            try
            {
                var batch = await batchService.CreateBatchAsync(createBatchDto);
                return Results.Created($"/api/batches/{batch.Id}", batch);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear lote");
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> UpdateBatch(
            Guid id,
            UpdateBatchDto updateBatchDto,
            ClaimsPrincipal user,
            IBatchService batchService,
            ILogger<IBatchService> logger)
        {
            try
            {
                var batch = await batchService.UpdateBatchAsync(id, updateBatchDto);
                if (batch == null)
                    return Results.NotFound("Lote no encontrado");

                return Results.Ok(batch);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar lote {Id}", id);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> MoveBatchQuantity(
            BatchMovementDto movementDto,
            ClaimsPrincipal user,
            IBatchService batchService,
            ILogger<IBatchService> logger)
        {
            try
            {
                var result = await batchService.MoveBatchQuantityAsync(movementDto);
                if (result)
                    return Results.Ok();

                return Results.BadRequest("Error al mover cantidad del lote");
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al mover cantidad del lote");
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> GetExpiredBatches(
            ClaimsPrincipal user,
            IBatchService batchService,
            ILogger<IBatchService> logger)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("ID de empresa no válido");

                var batches = await batchService.GetExpiredBatchesAsync(companyId);
                return Results.Ok(batches);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener lotes vencidos");
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> GetNearExpirationBatches(
            ClaimsPrincipal user,
            IBatchService batchService,
            ILogger<IBatchService> logger,
            int days = 30)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("ID de empresa no válido");

                var batches = await batchService.GetNearExpirationBatchesAsync(companyId, days);
                return Results.Ok(batches);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener lotes próximos a vencer");
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> GetAvailableQuantity(
            Guid productId,
            Guid warehouseId,
            ClaimsPrincipal user,
            IBatchService batchService,
            ILogger<IBatchService> logger)
        {
            try
            {
                var quantity = await batchService.GetAvailableQuantityAsync(productId, warehouseId);
                return Results.Ok(new { availableQuantity = quantity });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener cantidad disponible para producto {ProductId} en almacén {WarehouseId}", productId, warehouseId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static Guid GetCompanyId(ClaimsPrincipal user)
        {
            var companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            {
                throw new InvalidOperationException("ID de empresa no encontrado o no válido en los claims del usuario");
            }
            return companyId;
        }
    }
}