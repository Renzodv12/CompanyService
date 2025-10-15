using CompanyService.Core.Entities;
using CompanyService.Core.DTOs;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Company;
using CompanyService.Core.Utils;
using CompanyService.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.WebApi.Endpoints
{
    public static class RoleEndpoints
    {
        public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/companies/{companyId:guid}/roles", GetAllRoles)
               .WithName("GetAllRoles")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces<IEnumerable<RoleDto>>(StatusCodes.Status200OK);

            app.MapGet("/api/companies/{companyId:guid}/roles/{id:guid}", GetRoleById)
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

            app.MapPut("/api/companies/{companyId:guid}/roles/{id:guid}", UpdateRole)
               .WithName("UpdateRole")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Accepts<UpdateRoleRequest>("application/json")
               .Produces<RoleDto>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound)
               .Produces(StatusCodes.Status400BadRequest);

            app.MapDelete("/api/companies/{companyId:guid}/roles/{id:guid}", DeleteRole)
               .WithName("DeleteRole")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status404NotFound);

            app.MapGet("/api/companies/{companyId:guid}/roles/{id:guid}/permissions", GetRolePermissions)
               .WithName("GetRolePermissions")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces<IEnumerable<PermissionDto>>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound);

            app.MapPost("/api/companies/{companyId:guid}/roles/{id:guid}/permissions", AssignPermissionsToRole)
               .WithName("AssignPermissionsToRole")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Accepts<AssignPermissionsRequest>("application/json")
               .Produces(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound)
               .Produces(StatusCodes.Status400BadRequest);

            app.MapDelete("/api/companies/{companyId:guid}/roles/{roleId:guid}/permissions/{permissionId:guid}", RemovePermissionFromRole)
               .WithName("RemovePermissionFromRole")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status404NotFound);

            // Endpoint para asignar permisos con acciones específicas
            app.MapPost("/api/companies/{companyId:guid}/roles/{roleId:guid}/permissions/with-actions", AssignPermissionsWithActions)
               .WithName("AssignPermissionsWithActions")
               .WithTags("Roles")
               .WithOpenApi()
               .RequireAuthorization()
               .Accepts<AssignPermissionsWithActionsRequest>("application/json")
               .Produces(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status400BadRequest);

            return app;
        }

        private static async Task<IResult> GetAllRoles(Guid companyId, ApplicationDbContext context)
        {
            try
            {
                var roles = await context.Roles
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .Where(r => r.CompanyId == companyId)
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        CompanyId = r.CompanyId,
                        Permissions = r.RolePermissions.Select(rp => new PermissionDto
                        {
                            PermissionId = rp.PermissionId,
                            Key = rp.Permission.Key,
                            Description = rp.Permission.Description,
                            Actions = ((int)rp.Actions).GetPermissionsNames()
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

        private static async Task<IResult> GetRoleById(Guid companyId, Guid id, ApplicationDbContext context)
        {
            try
            {
                var role = await context.Roles
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .Where(r => r.Id == id && r.CompanyId == companyId)
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        CompanyId = r.CompanyId,
                        Permissions = r.RolePermissions.Select(rp => new PermissionDto
                        {
                            PermissionId = rp.PermissionId,
                            Key = rp.Permission.Key,
                            Description = rp.Permission.Description,
                            Actions = ((int)rp.Actions).GetPermissionsNames()
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (role == null)
                    return Results.NoContent();

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
                // Validar que la compañía existe
                var company = await context.Companies.FindAsync(request.CompanyId);
                if (company == null)
                    return Results.BadRequest(new { error = "La compañía especificada no existe" });

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

        private static async Task<IResult> UpdateRole(Guid companyId, Guid id, UpdateRoleRequest request, ApplicationDbContext context)
        {
            try
            {
                var role = await context.Roles
                    .FirstOrDefaultAsync(r => r.Id == id && r.CompanyId == companyId);
                    
                if (role == null)
                    return Results.NoContent();

                // Validar que no exista otro rol con el mismo nombre en la empresa
                var existingRole = await context.Roles
                    .FirstOrDefaultAsync(r => r.Name == request.Name && r.CompanyId == companyId && r.Id != id);

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

        private static async Task<IResult> DeleteRole(Guid companyId, Guid id, ApplicationDbContext context)
        {
            try
            {
                var role = await context.Roles
                    .Include(r => r.RolePermissions)
                    .Include(r => r.UserCompanies)
                    .FirstOrDefaultAsync(r => r.Id == id && r.CompanyId == companyId);

                if (role == null)
                    return Results.NoContent();

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

        private static async Task<IResult> GetRolePermissions(Guid companyId, Guid id, ApplicationDbContext context)
        {
            try
            {
                var role = await context.Roles
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(r => r.Id == id && r.CompanyId == companyId);

                if (role == null)
                    return Results.NoContent();

                var permissions = role.RolePermissions.Select(rp => new PermissionDto
                {
                    PermissionId = rp.PermissionId, 
                    Key = rp.Permission.Key,
                    Description = rp.Permission.Description,
                    Actions = ((int)rp.Actions).GetPermissionsNames()
                }).ToList();

                return Results.Ok(permissions);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al obtener permisos del rol", details = ex.Message });
            }
        }

        private static async Task<IResult> AssignPermissionsToRole(Guid companyId, Guid id, AssignPermissionsRequest request, ApplicationDbContext context)
        {
            try
            {
                var role = await context.Roles
                    .Include(r => r.RolePermissions)
                    .FirstOrDefaultAsync(r => r.Id == id && r.CompanyId == companyId);

                if (role == null)
                    return Results.NoContent();

                // Verificar que todos los permisos existan
                var permissions = await context.Permissions
                    .Where(p => request.PermissionIds.Contains(p.Id))
                    .ToListAsync();

                if (permissions.Count != request.PermissionIds.Count)
                    return Results.BadRequest(new { error = "Algunos permisos no existen" });

                // Eliminar permisos actuales
                context.RolePermissions.RemoveRange(role.RolePermissions);

                // Agregar nuevos permisos con solo View por defecto
                foreach (var permissionId in request.PermissionIds)
                {
                    role.RolePermissions.Add(new RolePermission
                    {
                        RoleId = id,
                        PermissionId = permissionId,
                        Actions = PermissionAction.View // Solo View por defecto, otras acciones se asignan explícitamente
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

        private static async Task<IResult> RemovePermissionFromRole(Guid companyId, Guid roleId, Guid permissionId, ApplicationDbContext context)
        {
            try
            {
                var rolePermission = await context.RolePermissions
                    .Include(rp => rp.Role)
                    .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId && rp.Role.CompanyId == companyId);

                if (rolePermission == null)
                    return Results.NoContent();

                context.RolePermissions.Remove(rolePermission);
                await context.SaveChangesAsync();

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al remover permiso del rol", details = ex.Message });
            }
        }

        private static async Task<IResult> AssignPermissionsWithActions(Guid companyId, Guid roleId, AssignPermissionsWithActionsRequest request, ApplicationDbContext context)
        {
            try
            {
                var role = await context.Roles
                    .Include(r => r.RolePermissions)
                    .FirstOrDefaultAsync(r => r.Id == roleId && r.CompanyId == companyId);

                if (role == null)
                    return Results.NoContent();

                // Verificar que todos los permisos existan
                var permissionIds = request.Permissions.Select(p => p.PermissionId).ToList();
                var permissions = await context.Permissions
                    .Where(p => permissionIds.Contains(p.Id))
                    .ToListAsync();

                if (permissions.Count != permissionIds.Count)
                    return Results.BadRequest(new { error = "Algunos permisos no existen" });

                // Eliminar permisos actuales de forma más segura
                var existingPermissions = await context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .ToListAsync();
                
                context.RolePermissions.RemoveRange(existingPermissions);
                
                // Guardar cambios para eliminar los permisos existentes
                await context.SaveChangesAsync();

                // Agregar nuevos permisos con acciones específicas
                foreach (var permissionAssignment in request.Permissions)
                {
                    var actions = PermissionAction.None;
                    
                    // Convertir strings a PermissionAction flags
                    foreach (var actionString in permissionAssignment.Actions)
                    {
                        if (Enum.TryParse<PermissionAction>(actionString, true, out var action))
                        {
                            actions |= action;
                        }
                    }

                    var newRolePermission = new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleId = roleId,
                        PermissionId = permissionAssignment.PermissionId,
                        Actions = actions
                    };
                    
                    context.RolePermissions.Add(newRolePermission);
                }

                await context.SaveChangesAsync();

                // Devolver información detallada de los permisos asignados
                var assignedPermissions = await context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .Include(rp => rp.Permission)
                    .Select(rp => new
                    {
                        RolePermissionId = rp.Id,
                        PermissionId = rp.PermissionId,
                        PermissionKey = rp.Permission.Key,
                        PermissionDescription = rp.Permission.Description,
                        Actions = ((int)rp.Actions).GetPermissionsNames()
                    })
                    .ToListAsync();

                return Results.Ok(new 
                { 
                    message = "Permisos asignados con acciones específicas correctamente",
                    roleId = roleId,
                    assignedPermissions = assignedPermissions
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al asignar permisos con acciones", details = ex.Message });
            }
        }
    }

}