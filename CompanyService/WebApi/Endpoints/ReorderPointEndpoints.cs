using CompanyService.Core.DTOs;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CompanyService.WebApi.Endpoints
{
    public static class ReorderPointEndpoints
    {
        public static void MapReorderPointEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/reorderpoint")
                .WithTags("ReorderPoints")
                .RequireAuthorization();

            // GET /api/reorderpoint/triggered
            group.MapGet("/triggered", GetTriggeredReorderPoints)
                .WithName("GetTriggeredReorderPoints")
                .WithSummary("Get triggered reorder points")
                .WithDescription("Retrieves all triggered reorder points for the authenticated user's company")
                .Produces<IEnumerable<ReorderPointResponseDto>>(200)
                .Produces(401);

            // POST /api/reorderpoint/{id}/trigger
            group.MapPost("/{id:guid}/trigger", TriggerReorderPoint)
                .WithName("TriggerReorderPoint")
                .WithSummary("Trigger reorder point")
                .WithDescription("Triggers a reorder point with current quantity")
                .Produces(200)
                .Produces(404);

            // POST /api/reorderpoint/{id}/mark-ordered
            group.MapPost("/{id:guid}/mark-ordered", MarkAsOrdered)
                .WithName("MarkAsOrdered")
                .WithSummary("Mark reorder point as ordered")
                .WithDescription("Marks a reorder point as ordered")
                .Produces(200)
                .Produces(404);

            // GET /api/reorderpoint/stats
            group.MapGet("/stats", GetReorderStats)
                .WithName("GetReorderStats")
                .WithSummary("Get reorder statistics")
                .WithDescription("Retrieves reorder statistics for the authenticated user's company")
                .Produces<object>(200)
                .Produces(401);

            // GET /api/reorderpoint
            group.MapGet("/", GetReorderPoints)
                .WithName("GetReorderPoints")
                .WithSummary("Get all reorder points")
                .WithDescription("Retrieves all reorder points for the authenticated user's company")
                .Produces<IEnumerable<ReorderPointDto>>(200)
                .Produces(401);

            // GET /api/reorderpoint/{id}
            group.MapGet("/{id:guid}", GetReorderPoint)
                .WithName("GetReorderPoint")
                .WithSummary("Get reorder point by ID")
                .WithDescription("Retrieves a specific reorder point by its ID")
                .Produces<ReorderPointDto>(200)
                .Produces(404)
                .Produces(401);

            // GET /api/reorderpoint/warehouse/{warehouseId}
            group.MapGet("/warehouse/{warehouseId:guid}", GetReorderPointsByWarehouse)
                .WithName("GetReorderPointsByWarehouse")
                .WithSummary("Get reorder points by warehouse")
                .WithDescription("Retrieves all reorder points for a specific warehouse")
                .Produces<IEnumerable<ReorderPointDto>>(200)
                .Produces(401);

            // GET /api/reorderpoint/alerts
            group.MapGet("/alerts", GetReorderPointAlerts)
                .WithName("GetReorderPointAlerts")
                .WithSummary("Get reorder point alerts")
                .WithDescription("Retrieves active reorder point alerts for the authenticated user's company")
                .Produces<IEnumerable<ReorderAlertDto>>(200)
                .Produces(401);

            // POST /api/reorderpoint
            group.MapPost("/", CreateReorderPoint)
                .WithName("CreateReorderPoint")
                .WithSummary("Create new reorder point")
                .WithDescription("Creates a new reorder point")
                .Produces<ReorderPointResponseDto>(201)
                .Produces(400);

            // PUT /api/reorderpoint/{id}
            group.MapPut("/{id:guid}", UpdateReorderPoint)
                .WithName("UpdateReorderPoint")
                .WithSummary("Update reorder point")
                .WithDescription("Updates an existing reorder point")
                .Produces<ReorderPointResponseDto>(200)
                .Produces(404)
                .Produces(400);

            // DELETE /api/reorderpoint/{id}
            group.MapDelete("/{id:guid}", DeleteReorderPoint)
                .WithName("DeleteReorderPoint")
                .WithSummary("Delete reorder point")
                .WithDescription("Deletes a reorder point")
                .Produces(204)
                .Produces(404);
        }

        private static Guid GetCompanyId(ClaimsPrincipal user)
        {
            var companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out Guid companyId))
            {
                throw new UnauthorizedAccessException("Company ID not found in token");
            }
            return companyId;
        }

        private static async Task<IResult> GetTriggeredReorderPoints(
            IReorderPointService reorderPointService,
            ClaimsPrincipal user)
        {
            try
            {
                var companyId = GetCompanyId(user);
                var triggeredPoints = await reorderPointService.GetTriggeredReorderPointsAsync(companyId);
                return Results.Ok(triggeredPoints);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        }

        private static async Task<IResult> TriggerReorderPoint(
            Guid id,
            decimal currentQuantity,
            IReorderPointService reorderPointService,
            ClaimsPrincipal user)
        {
            var success = await reorderPointService.TriggerReorderPointAsync(id, currentQuantity);
            
            if (!success)
                return Results.NoContent();

            return Results.Ok();
        }

        private static async Task<IResult> MarkAsOrdered(
            Guid id,
            IReorderPointService reorderPointService,
            ClaimsPrincipal user)
        {
            var success = await reorderPointService.MarkAsOrderedAsync(id);
            
            if (!success)
                return Results.NoContent();

            return Results.Ok();
        }

        private static async Task<IResult> GetReorderStats(
            IReorderPointService reorderPointService,
            ClaimsPrincipal user)
        {
            try
            {
                var companyId = GetCompanyId(user);
                var stats = await reorderPointService.GetReorderStatsAsync(companyId);
                return Results.Ok(stats);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        }

        private static async Task<IResult> GetReorderPoints(
            IReorderPointService reorderPointService,
            ClaimsPrincipal user)
        {
            try
            {
                var companyId = GetCompanyId(user);
                var reorderPoints = await reorderPointService.GetReorderPointsByCompanyAsync(companyId);
                return Results.Ok(reorderPoints);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        }

        private static async Task<IResult> GetReorderPoint(
            Guid id,
            IReorderPointService reorderPointService,
            ClaimsPrincipal user)
        {
            try
            {
                var companyId = GetCompanyId(user);
                var reorderPoint = await reorderPointService.GetReorderPointByIdAsync(id);
                
                if (reorderPoint == null)
                    return Results.NoContent();

                return Results.Ok(reorderPoint);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        }

        private static async Task<IResult> GetReorderPointsByWarehouse(
            Guid warehouseId,
            IReorderPointService reorderPointService,
            ClaimsPrincipal user)
        {
            try
            {
                var companyId = GetCompanyId(user);
                var reorderPoints = await reorderPointService.GetReorderPointsByWarehouseAsync(warehouseId);
                return Results.Ok(reorderPoints);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        }

        private static async Task<IResult> GetReorderPointAlerts(
            IReorderPointService reorderPointService,
            ClaimsPrincipal user)
        {
            try
            {
                var companyId = GetCompanyId(user);
                var alerts = await reorderPointService.GetActiveReorderAlertsAsync(companyId);
                return Results.Ok(alerts);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        }

        private static async Task<IResult> CreateReorderPoint(
            CreateReorderPointDto createReorderPointDto,
            IReorderPointService reorderPointService,
            ClaimsPrincipal user)
        {
            try
            {
                var reorderPoint = await reorderPointService.CreateReorderPointAsync(createReorderPointDto);
                return Results.Created($"/api/reorderpoint/{reorderPoint.Id}", reorderPoint);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> UpdateReorderPoint(
            Guid id,
            UpdateReorderPointDto updateReorderPointDto,
            IReorderPointService reorderPointService,
            ClaimsPrincipal user)
        {
            try
            {
                var reorderPoint = await reorderPointService.UpdateReorderPointAsync(id, updateReorderPointDto);
                
                if (reorderPoint == null)
                    return Results.NoContent();

                return Results.Ok(reorderPoint);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> DeleteReorderPoint(
            Guid id,
            IReorderPointService reorderPointService,
            ClaimsPrincipal user)
        {
            var success = await reorderPointService.DeleteReorderPointAsync(id);
            
            if (!success)
                return Results.NoContent();

            return Results.NoContent();
        }
    }
}