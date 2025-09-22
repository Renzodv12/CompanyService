using CompanyService.Core.DTOs;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CompanyService.WebApi.Endpoints
{
    public static class PhysicalInventoryEndpoints
    {
        public static void MapPhysicalInventoryEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/physicalinventory")
                .WithTags("PhysicalInventory")
                .RequireAuthorization();

            // Basic CRUD operations
            group.MapGet("/", GetPhysicalInventories)
                .WithName("GetPhysicalInventories")
                .WithOpenApi();

            group.MapGet("/{id:guid}", GetPhysicalInventory)
                .WithName("GetPhysicalInventory")
                .WithOpenApi();

            group.MapPost("/", CreatePhysicalInventory)
                .WithName("CreatePhysicalInventory")
                .WithOpenApi();

            group.MapPut("/{id:guid}", UpdatePhysicalInventory)
                .WithName("UpdatePhysicalInventory")
                .WithOpenApi();

            // Inventory Items operations
            group.MapGet("/{id:guid}/items", GetInventoryItems)
                .WithName("GetInventoryItems")
                .WithOpenApi();

            group.MapPut("/{id:guid}/items/{itemId:guid}", UpdatePhysicalInventoryItem)
                .WithName("UpdatePhysicalInventoryItem")
                .WithOpenApi();

            // Warehouse operations
            group.MapGet("/warehouse/{warehouseId:guid}", GetPhysicalInventoriesByWarehouse)
                .WithName("GetPhysicalInventoriesByWarehouse")
                .WithOpenApi();

            // Process operations
            group.MapPost("/{id:guid}/start", StartPhysicalInventory)
                .WithName("StartPhysicalInventory")
                .WithOpenApi();

            group.MapPost("/{id:guid}/complete", CompletePhysicalInventory)
                .WithName("CompletePhysicalInventory")
                .WithOpenApi();

            group.MapPost("/{id:guid}/cancel", CancelPhysicalInventory)
                .WithName("CancelPhysicalInventory")
                .WithOpenApi();

            // Reports
            group.MapGet("/{id:guid}/variance-report", GetInventoryVarianceReport)
                .WithName("GetInventoryVarianceReport")
                .WithOpenApi();
        }

        #region Basic CRUD Operations

        private static async Task<IResult> GetPhysicalInventories(
            ClaimsPrincipal user,
            IPhysicalInventoryService physicalInventoryService,
            ILogger<IPhysicalInventoryService> logger)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("Company ID not found in token");

                var inventories = await physicalInventoryService.GetPhysicalInventoriesByCompanyAsync(companyId);
                return Results.Ok(inventories);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting physical inventories");
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> GetPhysicalInventory(
            Guid id,
            IPhysicalInventoryService physicalInventoryService,
            ILogger<IPhysicalInventoryService> logger)
        {
            try
            {
                var inventory = await physicalInventoryService.GetPhysicalInventoryByIdAsync(id);
                if (inventory == null)
                    return Results.NoContent();

                return Results.Ok(inventory);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting physical inventory by ID: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> CreatePhysicalInventory(
            CreatePhysicalInventoryDto createDto,
            IPhysicalInventoryService physicalInventoryService,
            ILogger<IPhysicalInventoryService> logger)
        {
            try
            {
                var inventory = await physicalInventoryService.CreatePhysicalInventoryAsync(createDto);
                return Results.Created($"/api/physicalinventory/{inventory.Id}", inventory);
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
                logger.LogError(ex, "Error creating physical inventory");
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> UpdatePhysicalInventory(
            Guid id,
            UpdatePhysicalInventoryDto updateDto,
            IPhysicalInventoryService physicalInventoryService,
            ILogger<IPhysicalInventoryService> logger)
        {
            try
            {
                var inventory = await physicalInventoryService.UpdatePhysicalInventoryAsync(id, updateDto);
                if (inventory == null)
                    return Results.NoContent();

                return Results.Ok(inventory);
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
                logger.LogError(ex, "Error updating physical inventory: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        #endregion

        #region Inventory Items Operations

        private static async Task<IResult> GetInventoryItems(
            Guid id,
            IPhysicalInventoryService physicalInventoryService,
            ILogger<IPhysicalInventoryService> logger)
        {
            try
            {
                var items = await physicalInventoryService.GetInventoryItemsAsync(id);
                return Results.Ok(items);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting inventory items for inventory: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> UpdatePhysicalInventoryItem(
            Guid id,
            Guid itemId,
            UpdatePhysicalInventoryItemDto updateItemDto,
            IPhysicalInventoryService physicalInventoryService,
            ILogger<IPhysicalInventoryService> logger)
        {
            try
            {
                var item = await physicalInventoryService.UpdateInventoryItemAsync(id, itemId, updateItemDto);
                if (item == null)
                    return Results.NoContent();

                return Results.Ok(item);
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
                logger.LogError(ex, "Error updating inventory item: {ItemId} in inventory: {Id}", itemId, id);
                return Results.Problem("Internal server error");
            }
        }

        #endregion

        #region Warehouse Operations

        private static async Task<IResult> GetPhysicalInventoriesByWarehouse(
            Guid warehouseId,
            IPhysicalInventoryService physicalInventoryService,
            ILogger<IPhysicalInventoryService> logger)
        {
            try
            {
                var inventories = await physicalInventoryService.GetPhysicalInventoriesByWarehouseAsync(warehouseId);
                return Results.Ok(inventories);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting physical inventories by warehouse: {WarehouseId}", warehouseId);
                return Results.Problem("Internal server error");
            }
        }

        #endregion

        #region Process Operations

        private static async Task<IResult> StartPhysicalInventory(
            Guid id,
            IPhysicalInventoryService physicalInventoryService,
            ILogger<IPhysicalInventoryService> logger)
        {
            try
            {
                var success = await physicalInventoryService.StartPhysicalInventoryAsync(id);
                if (!success)
                    return Results.NoContent();

                return Results.Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error starting physical inventory: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> CompletePhysicalInventory(
            Guid id,
            IPhysicalInventoryService physicalInventoryService,
            ILogger<IPhysicalInventoryService> logger)
        {
            try
            {
                var success = await physicalInventoryService.CompletePhysicalInventoryAsync(id);
                if (!success)
                    return Results.NoContent();

                return Results.Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error completing physical inventory: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> CancelPhysicalInventory(
            Guid id,
            IPhysicalInventoryService physicalInventoryService,
            ILogger<IPhysicalInventoryService> logger)
        {
            try
            {
                var success = await physicalInventoryService.CancelPhysicalInventoryAsync(id);
                if (!success)
                    return Results.NoContent();

                return Results.Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error canceling physical inventory: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        #endregion

        #region Reports

        private static async Task<IResult> GetInventoryVarianceReport(
            Guid id,
            IPhysicalInventoryService physicalInventoryService,
            ILogger<IPhysicalInventoryService> logger)
        {
            try
            {
                var report = await physicalInventoryService.GetInventoryVarianceReportAsync(id);
                return Results.Ok(report);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting inventory variance report for inventory: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        #endregion

        #region Helper Methods

        private static Guid GetCompanyId(ClaimsPrincipal user)
        {
            var companyIdClaim = user.FindFirst("CompanyId")?.Value;
            return Guid.TryParse(companyIdClaim, out var companyId) ? companyId : Guid.Empty;
        }

        #endregion
    }
}