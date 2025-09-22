using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CompanyService.WebApi.Endpoints
{
    public static class QuotationEndpoints
    {
        public static void MapQuotationEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/quotations")
                .WithTags("Quotations")
                .RequireAuthorization();

            // GET /api/quotations
            group.MapGet("/", GetQuotations)
                .WithName("GetQuotations")
                .WithSummary("Get all quotations")
                .WithDescription("Retrieves all quotations with optional filtering")
                .Produces<IEnumerable<QuotationResponse>>(200)
                .Produces(400);

            // GET /api/quotations/{id}
            group.MapGet("/{id:guid}", GetQuotation)
                .WithName("GetQuotation")
                .WithSummary("Get quotation by ID")
                .WithDescription("Retrieves a specific quotation by its ID")
                .Produces<QuotationResponse>(200)
                .Produces(404)
                .Produces(400);

            // POST /api/quotations
            group.MapPost("/", CreateQuotation)
                .WithName("CreateQuotation")
                .WithSummary("Create new quotation")
                .WithDescription("Creates a new quotation")
                .Produces<QuotationResponse>(201)
                .Produces(400);

            // PUT /api/quotations/{id}
            group.MapPut("/{id:guid}", UpdateQuotation)
                .WithName("UpdateQuotation")
                .WithSummary("Update quotation")
                .WithDescription("Updates an existing quotation")
                .Produces<QuotationResponse>(200)
                .Produces(400)
                .Produces(404);

            // PATCH /api/quotations/{id}/status
            group.MapPatch("/{id:guid}/status", UpdateQuotationStatus)
                .WithName("UpdateQuotationStatus")
                .WithSummary("Update quotation status")
                .WithDescription("Updates the status of a quotation")
                .Produces<QuotationResponse>(200)
                .Produces(400)
                .Produces(404);

            // DELETE /api/quotations/{id}
            group.MapDelete("/{id:guid}", DeleteQuotation)
                .WithName("DeleteQuotation")
                .WithSummary("Delete quotation")
                .WithDescription("Deletes a quotation")
                .Produces(204)
                .Produces(400)
                .Produces(404);

            // POST /api/quotations/{id}/items
            group.MapPost("/{id:guid}/items", AddQuotationItem)
                .WithName("AddQuotationItem")
                .WithSummary("Add quotation item")
                .WithDescription("Adds an item to a quotation")
                .Produces<QuotationItemResponse>(200)
                .Produces(400)
                .Produces(404);

            // PUT /api/quotations/{id}/items/{itemId}
            group.MapPut("/{id:guid}/items/{itemId:guid}", UpdateQuotationItem)
                .WithName("UpdateQuotationItem")
                .WithSummary("Update quotation item")
                .WithDescription("Updates an item in a quotation")
                .Produces<QuotationItemResponse>(200)
                .Produces(400)
                .Produces(404);

            // DELETE /api/quotations/{id}/items/{itemId}
            group.MapDelete("/{id:guid}/items/{itemId:guid}", RemoveQuotationItem)
                .WithName("RemoveQuotationItem")
                .WithSummary("Remove quotation item")
                .WithDescription("Removes an item from a quotation")
                .Produces(204)
                .Produces(400)
                .Produces(404);

            // POST /api/quotations/{id}/convert-to-purchase-order
            group.MapPost("/{id:guid}/convert-to-purchase-order", ConvertToPurchaseOrder)
                .WithName("ConvertToPurchaseOrder")
                .WithSummary("Convert quotation to purchase order")
                .WithDescription("Converts a quotation to a purchase order")
                .Produces<PurchaseOrderResponse>(200)
                .Produces(400)
                .Produces(404);

            // POST /api/quotations/compare
            group.MapPost("/compare", CompareQuotations)
                .WithName("CompareQuotations")
                .WithSummary("Compare quotations")
                .WithDescription("Compares multiple quotations")
                .Produces<QuotationComparisonResponse>(200)
                .Produces(400);

            // GET /api/quotations/reports
            group.MapGet("/reports", GetQuotationReport)
                .WithName("GetQuotationReport")
                .WithSummary("Get quotation report")
                .WithDescription("Generates a quotation report")
                .Produces<QuotationReportResponse>(200)
                .Produces(400);
        }

        private static async Task<IResult> GetQuotations(
            [AsParameters] QuotationFilterRequest filter,
            IProcurementService procurementService,
            ClaimsPrincipal user)
        {
            try
            {
                var result = await procurementService.GetQuotationsAsync(filter.CompanyId);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> GetQuotation(
            Guid id,
            IProcurementService procurementService,
            ClaimsPrincipal user)
        {
            try
            {
                var result = await procurementService.GetQuotationByIdAsync(id);
                if (result == null)
                    return Results.NoContent();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> CreateQuotation(
            CreateQuotationRequest request,
            IProcurementService procurementService,
            ClaimsPrincipal user)
        {
            try
            {
                // Convert DTO to entity - this needs proper mapping
                var quotation = new Core.Entities.Quotation
                {
                    CompanyId = new Guid(), // TODO: Fix CompanyId mapping from int to Guid
                    SupplierId = new Guid(), // TODO: Fix SupplierId mapping from int to Guid
                    QuotationNumber = request.QuotationNumber ?? string.Empty,
                    // Add other required properties
                };
                var result = await procurementService.CreateQuotationAsync(quotation);
                return Results.Created($"/api/quotations/{result.Id}", result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> UpdateQuotation(
            Guid id,
            UpdateQuotationRequest request,
            IProcurementService procurementService,
            ClaimsPrincipal user)
        {
            try
            {
                // Convert DTO to entity - this needs proper mapping
                var quotation = new Core.Entities.Quotation
                {
                    Id = id,
                    // Map other properties from request
                };
                var result = await procurementService.UpdateQuotationAsync(id, quotation);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> UpdateQuotationStatus(
            Guid id,
            UpdateQuotationStatusRequest request,
            IProcurementService procurementService,
            ClaimsPrincipal user)
        {
            try
            {
                var result = await procurementService.UpdateQuotationStatusAsync(id, request.Status);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> DeleteQuotation(
            Guid id,
            IProcurementService procurementService,
            ClaimsPrincipal user)
        {
            try
            {
                await procurementService.DeleteQuotationAsync(id);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> AddQuotationItem(
            Guid id,
            CreateQuotationItemRequest request,
            IProcurementService procurementService,
            ClaimsPrincipal user)
        {
            try
            {
                // Convert DTO to entity
                var item = new Core.Entities.QuotationItem
                {
                    QuotationId = id,
                    ProductId = new Guid(), // TODO: Fix ProductId mapping from int to Guid
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    // Map other properties
                };
                var result = await procurementService.AddQuotationItemAsync(item);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> UpdateQuotationItem(
            Guid id,
            Guid itemId,
            UpdateQuotationItemRequest request,
            IProcurementService procurementService,
            ClaimsPrincipal user)
        {
            try
            {
                // Convert DTO to entity
                var item = new Core.Entities.QuotationItem
                {
                    Id = itemId,
                    QuotationId = id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    // Map other properties
                };
                var result = await procurementService.UpdateQuotationItemAsync(item);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> RemoveQuotationItem(
            Guid id,
            Guid itemId,
            IProcurementService procurementService,
            ClaimsPrincipal user)
        {
            try
            {
                await procurementService.RemoveQuotationItemAsync(itemId);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> ConvertToPurchaseOrder(
            Guid id,
            ConvertQuotationToPurchaseOrderRequest request,
            IProcurementService procurementService,
            ClaimsPrincipal user)
        {
            try
            {
                // Get user ID from claims
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.BadRequest(new { message = "Invalid user ID" });
                }

                var result = await procurementService.ConvertQuotationToPurchaseOrderAsync(id, userId);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> CompareQuotations(
            QuotationComparisonRequest request,
            IProcurementService procurementService,
            ClaimsPrincipal user)
        {
            try
            {
                var result = await procurementService.CompareQuotationsAsync(request);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> GetQuotationReport(
            [AsParameters] QuotationReportRequest request,
            IProcurementService procurementService,
            ClaimsPrincipal user)
        {
            try
            {
                var result = await procurementService.GetQuotationReportAsync(request);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }
    }
}