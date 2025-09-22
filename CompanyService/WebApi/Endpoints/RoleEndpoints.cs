using CompanyService.Core.Entities;
using CompanyService.Core.DTOs;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.WebApi.Endpoints
{
    public static class RoleEndpoints
    {
        public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/roles", GetAllRoles)
               .WithName("GetAllRoles")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces<IEnumerable<RoleDto>>(StatusCodes.Status200OK);

            app.MapGet("/api/roles/{id:guid}", GetRoleById)
               .WithName("GetRoleById")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces<RoleDto>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound);

            app.MapPost("/api/roles", CreateRole)
               .WithName("CreateRole")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Accepts<CreateRoleRequest>("application/json")
               .Produces<RoleDto>(StatusCodes.Status201Created)
               .Produces(StatusCodes.Status400BadRequest);

            app.MapPut("/api/roles/{id:guid}", UpdateRole)
               .WithName("UpdateRole")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Accepts<UpdateRoleRequest>("application/json")
               .Produces<RoleDto>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound)
               .Produces(StatusCodes.Status400BadRequest);

            app.MapDelete("/api/roles/{id:guid}", DeleteRole)
               .WithName("DeleteRole")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status404NotFound);

            app.MapGet("/api/roles/{id:guid}/permissions", GetRolePermissions)
               .WithName("GetRolePermissions")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces<IEnumerable<PermissionDto>>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound);

            app.MapPost("/api/roles/{id:guid}/permissions", AssignPermissionsToRole)
               .WithName("AssignPermissionsToRole")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Accepts<AssignPermissionsRequest>("application/json")
               .Produces(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound)
               .Produces(StatusCodes.Status400BadRequest);

            app.MapDelete("/api/roles/{roleId:guid}/permissions/{permissionId:guid}", RemovePermissionFromRole)
               .WithName("RemovePermissionFromRole")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status404NotFound);

            return app;
        }

        private static async Task<IResult> GetAllRoles(ApplicationDbContext context)
        {
            try
            {
                var roles = await context.Roles
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        CompanyId = r.CompanyId,
                        Permissions = r.RolePermissions.Select(rp => new PermissionDto
                        {
                            Id = rp.Permission.Id,
                            Key = rp.Permission.Key,
                            Description = rp.Permission.Description
                        }).ToList()
                    })
                    .ToListAsync();

                return Results.Ok(roles);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al obtener roles", details = ex.Message });
            }
        }

        private static async Task<IResult> GetRoleById(Guid id, ApplicationDbContext context)
        {
            try
            {
                var role = await context.Roles
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .Where(r => r.Id == id)
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        CompanyId = r.CompanyId,
                        Permissions = r.RolePermissions.Select(rp => new PermissionDto
                        {
                            Id = rp.Permission.Id,
                            Key = rp.Permission.Key,
                            Description = rp.Permission.Description
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (role == null)
                    return Results.NotFound(new { error = "Rol no encontrado" });

                return Results.Ok(role);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al obtener rol", details = ex.Message });
            }
        }

        private static async Task<IResult> CreateRole(CreateRoleRequest request, ApplicationDbContext context)
        {
            try
            {
                // Validar que no exista un rol con el mismo nombre en la empresa
                var existingRole = await context.Roles
                    .FirstOrDefaultAsync(r => r.Name == request.Name && r.CompanyId == request.CompanyId);

                if (existingRole != null)
                    return Results.BadRequest(new { error = "Ya existe un rol con ese nombre en la empresa" });

                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    CompanyId = request.CompanyId
                };

                context.Roles.Add(role);
                await context.SaveChangesAsync();

                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    CompanyId = role.CompanyId,
                    Permissions = new List<PermissionDto>()
                };

                return Results.Created($"/api/roles/{role.Id}", roleDto);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al crear rol", details = ex.Message });
            }
        }

        private static async Task<IResult> UpdateRole(Guid id, UpdateRoleRequest request, ApplicationDbContext context)
        {
            try
            {
                var role = await context.Roles.FindAsync(id);
                if (role == null)
                    return Results.NotFound(new { error = "Rol no encontrado" });

                // Validar que no exista otro rol con el mismo nombre en la empresa
                var existingRole = await context.Roles
                    .FirstOrDefaultAsync(r => r.Name == request.Name && r.CompanyId == role.CompanyId && r.Id != id);

                if (existingRole != null)
                    return Results.BadRequest(new { error = "Ya existe otro rol con ese nombre en la empresa" });

                role.Name = request.Name;
                role.Description = request.Description;

                await context.SaveChangesAsync();

                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    CompanyId = role.CompanyId,
                    Permissions = new List<PermissionDto>()
                };

                return Results.Ok(roleDto);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al actualizar rol", details = ex.Message });
            }
        }

        private static async Task<IResult> DeleteRole(Guid id, ApplicationDbContext context)
        {
            try
            {
                var role = await context.Roles
                    .Include(r => r.RolePermissions)
                    .Include(r => r.UserCompanies)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (role == null)
                    return Results.NotFound(new { error = "Rol no encontrado" });

                // Verificar si hay usuarios asignados a este rol
                if (role.UserCompanies.Any())
                    return Results.BadRequest(new { error = "No se puede eliminar el rol porque tiene usuarios asignados" });

                // Eliminar las relaciones de permisos
                context.RolePermissions.RemoveRange(role.RolePermissions);
                
                // Eliminar el rol
                context.Roles.Remove(role);
                await context.SaveChangesAsync();

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al eliminar rol", details = ex.Message });
            }
        }

        private static async Task<IResult> GetRolePermissions(Guid id, ApplicationDbContext context)
        {
            try
            {
                var role = await context.Roles
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (role == null)
                    return Results.NotFound(new { error = "Rol no encontrado" });

                var permissions = role.RolePermissions.Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Key = rp.Permission.Key,
                    Description = rp.Permission.Description
                }).ToList();

                return Results.Ok(permissions);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al obtener permisos del rol", details = ex.Message });
            }
        }

        private static async Task<IResult> AssignPermissionsToRole(Guid id, AssignPermissionsRequest request, ApplicationDbContext context)
        {
            try
            {
                var role = await context.Roles
                    .Include(r => r.RolePermissions)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (role == null)
                    return Results.NotFound(new { error = "Rol no encontrado" });

                // Verificar que todos los permisos existan
                var permissions = await context.Permissions
                    .Where(p => request.PermissionIds.Contains(p.Id))
                    .ToListAsync();

                if (permissions.Count != request.PermissionIds.Count)
                    return Results.BadRequest(new { error = "Algunos permisos no existen" });

                // Eliminar permisos actuales
                context.RolePermissions.RemoveRange(role.RolePermissions);

                // Agregar nuevos permisos
                foreach (var permissionId in request.PermissionIds)
                {
                    role.RolePermissions.Add(new RolePermission
                    {
                        RoleId = id,
                        PermissionId = permissionId
                    });
                }

                await context.SaveChangesAsync();

                return Results.Ok(new { message = "Permisos asignados correctamente" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al asignar permisos", details = ex.Message });
            }
        }

        private static async Task<IResult> RemovePermissionFromRole(Guid roleId, Guid permissionId, ApplicationDbContext context)
        {
            try
            {
                var rolePermission = await context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

                if (rolePermission == null)
                    return Results.NotFound(new { error = "Relación rol-permiso no encontrada" });

                context.RolePermissions.Remove(rolePermission);
                await context.SaveChangesAsync();

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al remover permiso del rol", details = ex.Message });
            }
        }
    }

    // DTOs para los endpoints de roles
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CompanyId { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new();
    }

    public class CreateRoleRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class UpdateRoleRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class AssignPermissionsRequest
    {
        public List<Guid> PermissionIds { get; set; } = new();
    }
}