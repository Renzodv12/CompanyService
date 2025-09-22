using CompanyService.Core.DTOs;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CompanyService.WebApi.Endpoints
{
    public static class WarehouseEndpoints
    {
        public static void MapWarehouseEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/warehouses")
                .WithTags("Warehouses")
                .RequireAuthorization();

            // GET /api/warehouses/{companyId}
            group.MapGet("/{companyId:guid}", GetWarehouses)
                .WithName("GetWarehouses")
                .WithSummary("Get warehouses by company")
                .WithDescription("Retrieves all warehouses for a specific company")
                .Produces<IEnumerable<WarehouseResponseDto>>(200)
                .Produces(500);

            // GET /api/warehouses/{companyId}/{id}
            group.MapGet("/{companyId:guid}/{id:guid}", GetWarehouse)
                .WithName("GetWarehouse")
                .WithSummary("Get warehouse by ID")
                .WithDescription("Retrieves a specific warehouse by its ID and company ID")
                .Produces<WarehouseResponseDto>(200)
                .Produces(404)
                .Produces(500);

            // POST /api/warehouses/{companyId}
            group.MapPost("/{companyId:guid}", CreateWarehouse)
                .WithName("CreateWarehouse")
                .WithSummary("Create new warehouse")
                .WithDescription("Creates a new warehouse for a specific company")
                .Produces<WarehouseResponseDto>(201)
                .Produces(400)
                .Produces(500);

            // PUT /api/warehouses/{companyId}/{id}
            group.MapPut("/{companyId:guid}/{id:guid}", UpdateWarehouse)
                .WithName("UpdateWarehouse")
                .WithSummary("Update warehouse")
                .WithDescription("Updates an existing warehouse")
                .Produces<WarehouseResponseDto>(200)
                .Produces(400)
                .Produces(404)
                .Produces(500);

            // DELETE /api/warehouses/{id}
            group.MapDelete("/{id:guid}", DeleteWarehouse)
                .WithName("DeleteWarehouse")
                .WithSummary("Delete warehouse")
                .WithDescription("Deletes a warehouse")
                .Produces(204)
                .Produces(400)
                .Produces(404)
                .Produces(500);

            // GET /api/warehouses/active
            group.MapGet("/active", GetActiveWarehouses)
                .WithName("GetActiveWarehouses")
                .WithSummary("Get active warehouses")
                .WithDescription("Retrieves all active warehouses for the authenticated user's company")
                .Produces<IEnumerable<WarehouseResponseDto>>(200)
                .Produces(401)
                .Produces(500);
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

        private static async Task<IResult> GetWarehouses(
            Guid companyId,
            IWarehouseService warehouseService,
            ClaimsPrincipal user)
        {
            try
            {
                var warehouses = await warehouseService.GetWarehousesByCompanyAsync(companyId);
                return Results.Ok(warehouses);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    title: "Error interno del servidor",
                    statusCode: 500);
            }
        }

        private static async Task<IResult> GetWarehouse(
            Guid companyId,
            Guid id,
            IWarehouseService warehouseService,
            ClaimsPrincipal user)
        {
            try
            {
                var warehouse = await warehouseService.GetWarehouseByIdAsync(id);
                
                if (warehouse == null)
                {
                    return Results.NotFound(new { message = "Almacén no encontrado" });
                }

                return Results.Ok(warehouse);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    title: "Error interno del servidor",
                    statusCode: 500);
            }
        }

        private static async Task<IResult> CreateWarehouse(
            Guid companyId,
            CreateWarehouseDto createWarehouseDto,
            IWarehouseService warehouseService,
            ClaimsPrincipal user)
        {
            try
            {
                createWarehouseDto.CompanyId = companyId;
                var warehouse = await warehouseService.CreateWarehouseAsync(createWarehouseDto);
                return Results.Created($"/api/warehouses/{companyId}/{warehouse.Id}", warehouse);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    title: "Error interno del servidor",
                    statusCode: 500);
            }
        }

        private static async Task<IResult> UpdateWarehouse(
            Guid companyId,
            Guid id,
            UpdateWarehouseDto updateWarehouseDto,
            IWarehouseService warehouseService,
            ClaimsPrincipal user)
        {
            try
            {
                var warehouse = await warehouseService.UpdateWarehouseAsync(id, updateWarehouseDto);
                
                if (warehouse == null)
                {
                    return Results.NotFound(new { message = "Almacén no encontrado" });
                }

                return Results.Ok(warehouse);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    title: "Error interno del servidor",
                    statusCode: 500);
            }
        }

        private static async Task<IResult> DeleteWarehouse(
            Guid id,
            IWarehouseService warehouseService,
            ClaimsPrincipal user)
        {
            try
            {
                var result = await warehouseService.DeleteWarehouseAsync(id);
                
                if (!result)
                {
                    return Results.NotFound(new { message = "Almacén no encontrado" });
                }

                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    title: "Error interno del servidor",
                    statusCode: 500);
            }
        }

        private static async Task<IResult> GetActiveWarehouses(
            IWarehouseService warehouseService,
            ClaimsPrincipal user)
        {
            try
            {
                var companyId = GetCompanyId(user);
                var warehouses = await warehouseService.GetActiveWarehousesByCompanyAsync(companyId);
                return Results.Ok(warehouses);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    title: "Error interno del servidor",
                    statusCode: 500);
            }
        }
    }
}