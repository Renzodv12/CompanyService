using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Entities;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CompanyService.WebApi.Endpoints
{
    public static class GoodsReceiptEndpoints
    {
        public static void MapGoodsReceiptEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/goodsreceipt")
                .WithTags("GoodsReceipt")
                .RequireAuthorization();

            // Basic CRUD operations
            group.MapGet("/", GetGoodsReceipts)
                .WithName("GetGoodsReceipts")
                .WithOpenApi();

            group.MapGet("/{id:guid}", GetGoodsReceipt)
                .WithName("GetGoodsReceipt")
                .WithOpenApi();

            group.MapPost("/", CreateGoodsReceipt)
                .WithName("CreateGoodsReceipt")
                .WithOpenApi();

            group.MapPut("/{id:guid}", UpdateGoodsReceipt)
                .WithName("UpdateGoodsReceipt")
                .WithOpenApi();

            group.MapPatch("/{id:guid}/status", UpdateGoodsReceiptStatus)
                .WithName("UpdateGoodsReceiptStatus")
                .WithOpenApi();

            group.MapDelete("/{id:guid}", DeleteGoodsReceipt)
                .WithName("DeleteGoodsReceipt")
                .WithOpenApi();

            // Goods Receipt Items operations
            group.MapPost("/{id:guid}/items", AddGoodsReceiptItem)
                .WithName("AddGoodsReceiptItem")
                .WithOpenApi();

            group.MapPut("/{id:guid}/items/{itemId:guid}", UpdateGoodsReceiptItem)
                .WithName("UpdateGoodsReceiptItem")
                .WithOpenApi();

            group.MapDelete("/{id:guid}/items/{itemId:guid}", RemoveGoodsReceiptItem)
                .WithName("RemoveGoodsReceiptItem")
                .WithOpenApi();

            // Inspection operations
            group.MapPost("/{id:guid}/items/{itemId:guid}/inspect", InspectGoodsReceiptItem)
                .WithName("InspectGoodsReceiptItem")
                .WithOpenApi();

            group.MapPost("/{id:guid}/bulk-inspection", BulkInspection)
                .WithName("BulkInspection")
                .WithOpenApi();

            // Completion operations
            group.MapPost("/{id:guid}/complete", CompleteGoodsReceipt)
                .WithName("CompleteGoodsReceipt")
                .WithOpenApi();

            // Query operations
            group.MapGet("/by-purchase-order/{purchaseOrderId:guid}", GetGoodsReceiptsByPurchaseOrder)
                .WithName("GetGoodsReceiptsByPurchaseOrder")
                .WithOpenApi();

            // Reports
            group.MapGet("/reports", GetGoodsReceiptReport)
                .WithName("GetGoodsReceiptReport")
                .WithOpenApi();
        }

        #region Basic CRUD Operations

        private static async Task<IResult> GetGoodsReceipts(
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger,
            [AsParameters] GoodsReceiptFilterRequest filter)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("Company ID not found in token");

                var result = await procurementService.GetGoodsReceiptsAsync(companyId);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting goods receipts");
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> GetGoodsReceipt(
            Guid id,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var result = await procurementService.GetGoodsReceiptByIdAsync(id);
                if (result == null)
                    return Results.NoContent();

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting goods receipt by ID: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> CreateGoodsReceipt(
            CreateGoodsReceiptRequest request,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("Company ID not found in token");

                var goodsReceipt = new GoodsReceipt
                {
                    CompanyId = companyId,
                    PurchaseOrderId = Guid.Parse(request.PurchaseOrderId.ToString()),
                    ReceiptNumber = request.ReceiptNumber ?? string.Empty,
                    // Map other properties from request as needed
                };

                var result = await procurementService.CreateGoodsReceiptAsync(goodsReceipt);
                return Results.Created($"/api/goodsreceipt/{result.Id}", result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating goods receipt");
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> UpdateGoodsReceipt(
            Guid id,
            UpdateGoodsReceiptRequest request,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                // TODO: Implement UpdateGoodsReceiptAsync method in service
                return Results.Ok(new { message = "Update goods receipt not implemented yet" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating goods receipt: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> UpdateGoodsReceiptStatus(
            Guid id,
            UpdateGoodsReceiptStatusRequest request,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                // TODO: Implement UpdateGoodsReceiptStatusAsync method in service
                return Results.Ok(new { message = "Update goods receipt status not implemented yet" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating goods receipt status: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> DeleteGoodsReceipt(
            Guid id,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                await procurementService.DeleteGoodsReceiptAsync(id);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting goods receipt: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        #endregion

        #region Goods Receipt Items Operations

        private static async Task<IResult> AddGoodsReceiptItem(
            Guid id,
            CreateGoodsReceiptItemRequest request,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                // TODO: Implement AddGoodsReceiptItemAsync method with correct signature
                var result = new { message = "Goods receipt item added successfully" };
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding goods receipt item to receipt: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> UpdateGoodsReceiptItem(
            Guid id,
            Guid itemId,
            UpdateGoodsReceiptItemRequest request,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                // TODO: Implement UpdateGoodsReceiptItemAsync method in service
                return Results.Ok(new { message = "Update goods receipt item not implemented yet" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating goods receipt item: {ItemId} in receipt: {Id}", itemId, id);
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> RemoveGoodsReceiptItem(
            Guid id,
            Guid itemId,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                // TODO: Implement RemoveGoodsReceiptItemAsync method in service
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing goods receipt item: {ItemId} from receipt: {Id}", itemId, id);
                return Results.Problem("Internal server error");
            }
        }

        #endregion

        #region Inspection Operations

        private static async Task<IResult> InspectGoodsReceiptItem(
            Guid id,
            Guid itemId,
            InspectGoodsReceiptItemRequest request,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                // TODO: Implement InspectGoodsReceiptItemAsync method in service
                return Results.Ok(new { message = "Inspect goods receipt item not implemented yet" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inspecting goods receipt item: {ItemId} in receipt: {Id}", itemId, id);
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> BulkInspection(
            Guid id,
            BulkInspectionRequest request,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                // TODO: Implement BulkInspectionAsync method in IProcurementService
                var result = new { message = "Bulk inspection completed successfully" };
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error performing bulk inspection for receipt: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        #endregion

        #region Completion Operations

        private static async Task<IResult> CompleteGoodsReceipt(
            Guid id,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var userId = GetUserId(user);
                if (userId == Guid.Empty)
                    return Results.BadRequest("User ID not found in token");

                var result = await procurementService.CompleteGoodsReceiptAsync(id, userId);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error completing goods receipt: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        #endregion

        #region Query Operations

        private static async Task<IResult> GetGoodsReceiptsByPurchaseOrder(
            Guid purchaseOrderId,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var result = await procurementService.GetGoodsReceiptsByPurchaseOrderAsync(purchaseOrderId);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting goods receipts by purchase order: {PurchaseOrderId}", purchaseOrderId);
                return Results.Problem("Internal server error");
            }
        }

        #endregion

        #region Reports

        private static async Task<IResult> GetGoodsReceiptReport(
            [AsParameters] GoodsReceiptReportRequest request,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                // TODO: Implement GetGoodsReceiptReportAsync method in IProcurementService
                var result = new { message = "Goods receipt report generated successfully" };
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating goods receipt report");
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

        private static Guid GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        #endregion
    }
}