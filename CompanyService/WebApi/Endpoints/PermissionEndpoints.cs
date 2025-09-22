using CompanyService.Core.Entities;
using CompanyService.Core.DTOs;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.WebApi.Endpoints
{
    public static class PermissionEndpoints
    {
        public static IEndpointRouteBuilder MapPermissionEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/permissions", GetAllPermissions)
               .WithName("GetAllPermissions")
               .WithTags("Permissions")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces<IEnumerable<PermissionDto>>(StatusCodes.Status200OK);

            app.MapGet("/api/permissions/{id:guid}", GetPermissionById)
               .WithName("GetPermissionById")
               .WithTags("Permissions")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces<PermissionDto>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound);

            app.MapPost("/api/permissions", CreatePermission)
               .WithName("CreatePermission")
               .WithTags("Permissions")
               .WithOpenApi()
               .RequireAuthorization()
               .Accepts<CreatePermissionRequest>("application/json")
               .Produces<PermissionDto>(StatusCodes.Status201Created)
               .Produces(StatusCodes.Status400BadRequest);

            app.MapPut("/api/permissions/{id:guid}", UpdatePermission)
               .WithName("UpdatePermission")
               .WithTags("Permissions")
               .WithOpenApi()
               .RequireAuthorization()
               .Accepts<UpdatePermissionRequest>("application/json")
               .Produces<PermissionDto>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound)
               .Produces(StatusCodes.Status400BadRequest);

            app.MapDelete("/api/permissions/{id:guid}", DeletePermission)
               .WithName("DeletePermission")
               .WithTags("Permissions")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status404NotFound);

            app.MapGet("/api/permissions/{id:guid}/roles", GetPermissionRoles)
               .WithName("GetPermissionRoles")
               .WithTags("Permissions")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces<IEnumerable<RoleDto>>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound);

            return app;
        }

        private static async Task<IResult> GetAllPermissions(ApplicationDbContext context)
        {
            try
            {
                var permissions = await context.Permissions
                    .Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Key = p.Key,
                        Description = p.Description
                    })
                    .ToListAsync();

                return Results.Ok(permissions);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al obtener permisos", details = ex.Message });
            }
        }

        private static async Task<IResult> GetPermissionById(Guid id, ApplicationDbContext context)
        {
            try
            {
                var permission = await context.Permissions
                    .Where(p => p.Id == id)
                    .Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Key = p.Key,
                        Description = p.Description
                    })
                    .FirstOrDefaultAsync();

                if (permission == null)
                    return Results.NotFound(new { error = "Permiso no encontrado" });

                return Results.Ok(permission);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al obtener permiso", details = ex.Message });
            }
        }

        private static async Task<IResult> CreatePermission(CreatePermissionRequest request, ApplicationDbContext context)
        {
            try
            {
                // Validar que no exista un permiso con la misma clave
                var existingPermission = await context.Permissions
                    .FirstOrDefaultAsync(p => p.Key == request.Key);

                if (existingPermission != null)
                    return Results.BadRequest(new { error = "Ya existe un permiso con esa clave" });

                var permission = new Permission
                {
                    Id = Guid.NewGuid(),
                    Key = request.Key,
                    Description = request.Description
                };

                context.Permissions.Add(permission);
                await context.SaveChangesAsync();

                var permissionDto = new PermissionDto
                {
                    Id = permission.Id,
                    Key = permission.Key,
                    Description = permission.Description
                };

                return Results.Created($"/api/permissions/{permission.Id}", permissionDto);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al crear permiso", details = ex.Message });
            }
        }

        private static async Task<IResult> UpdatePermission(Guid id, UpdatePermissionRequest request, ApplicationDbContext context)
        {
            try
            {
                var permission = await context.Permissions.FindAsync(id);
                if (permission == null)
                    return Results.NotFound(new { error = "Permiso no encontrado" });

                // Validar que no exista otro permiso con la misma clave
                var existingPermission = await context.Permissions
                    .FirstOrDefaultAsync(p => p.Key == request.Key && p.Id != id);

                if (existingPermission != null)
                    return Results.BadRequest(new { error = "Ya existe otro permiso con esa clave" });

                permission.Key = request.Key;
                permission.Description = request.Description;

                await context.SaveChangesAsync();

                var permissionDto = new PermissionDto
                {
                    Id = permission.Id,
                    Key = permission.Key,
                    Description = permission.Description
                };

                return Results.Ok(permissionDto);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al actualizar permiso", details = ex.Message });
            }
        }

        private static async Task<IResult> DeletePermission(Guid id, ApplicationDbContext context)
        {
            try
            {
                var permission = await context.Permissions
                    .Include(p => p.RolePermissions)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (permission == null)
                    return Results.NotFound(new { error = "Permiso no encontrado" });

                // Verificar si hay roles asignados a este permiso
                if (permission.RolePermissions.Any())
                    return Results.BadRequest(new { error = "No se puede eliminar el permiso porque está asignado a roles" });

                context.Permissions.Remove(permission);
                await context.SaveChangesAsync();

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al eliminar permiso", details = ex.Message });
            }
        }

        private static async Task<IResult> GetPermissionRoles(Guid id, ApplicationDbContext context)
        {
            try
            {
                var permission = await context.Permissions
                    .Include(p => p.RolePermissions)
                    .ThenInclude(rp => rp.Role)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (permission == null)
                    return Results.NotFound(new { error = "Permiso no encontrado" });

                var roles = permission.RolePermissions.Select(rp => new RoleDto
                {
                    Id = rp.Role.Id,
                    Name = rp.Role.Name,
                    Description = rp.Role.Description,
                    CompanyId = rp.Role.CompanyId,
                    Permissions = new List<PermissionDto>()
                }).ToList();

                return Results.Ok(roles);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al obtener roles del permiso", details = ex.Message });
            }
        }
    }

    // DTOs para los endpoints de permisos
    public class PermissionDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
    }

    public class CreatePermissionRequest
    {
        public string Key { get; set; }
        public string Description { get; set; }
    }

    public class UpdatePermissionRequest
    {
        public string Key { get; set; }
        public string Description { get; set; }
    }
}