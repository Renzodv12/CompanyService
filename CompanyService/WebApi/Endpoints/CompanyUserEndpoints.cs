using CompanyService.Core.Entities;
using CompanyService.Core.DTOs;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Company;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.WebApi.Endpoints
{
    public static class CompanyUserEndpoints
    {
        public static IEndpointRouteBuilder MapCompanyUserEndpoints(this IEndpointRouteBuilder app)
        {
            // Obtener usuarios de una compañía
            app.MapGet("/api/companies/{companyId:guid}/users", GetCompanyUsers)
               .WithName("GetCompanyUsers")
               .WithTags("CompanyUsers")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces<IEnumerable<CompanyUserDto>>(StatusCodes.Status200OK);

            // Obtener usuario específico de una compañía
            app.MapGet("/api/companies/{companyId:guid}/users/{userId:guid}", GetCompanyUser)
               .WithName("GetCompanyUser")
               .WithTags("CompanyUsers")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces<CompanyUserDto>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status204NoContent);

            // Asignar usuario a compañía
            app.MapPost("/api/companies/{companyId:guid}/users", AssignUserToCompany)
               .WithName("AssignUserToCompany")
               .WithTags("CompanyUsers")
               .WithOpenApi()
               .RequireAuthorization()
               .Accepts<AssignUserToCompanyRequest>("application/json")
               .Produces<CompanyUserDto>(StatusCodes.Status201Created)
               .Produces(StatusCodes.Status400BadRequest);

            // Cambiar rol de usuario en compañía
            app.MapPut("/api/companies/{companyId:guid}/users/{userId:guid}/role", ChangeUserRoleInCompany)
               .WithName("ChangeUserRoleInCompany")
               .WithTags("CompanyUsers")
               .WithOpenApi()
               .RequireAuthorization()
               .Accepts<ChangeUserRoleRequest>("application/json")
               .Produces<CompanyUserDto>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status400BadRequest);

            // Remover usuario de compañía
            app.MapDelete("/api/companies/{companyId:guid}/users/{userId:guid}", RemoveUserFromCompany)
               .WithName("RemoveUserFromCompany")
               .WithTags("CompanyUsers")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status400BadRequest);

            return app;
        }

        private static async Task<IResult> GetCompanyUsers(Guid companyId, ApplicationDbContext context)
        {
            try
            {
                var companyUsers = await context.UserCompanys
                    .Include(uc => uc.User)
                    .Include(uc => uc.Role)
                    .Where(uc => uc.CompanyId == companyId)
                    .Select(uc => new CompanyUserDto
                    {
                        Id = uc.Id,
                        UserId = uc.UserId,
                        CompanyId = uc.CompanyId,
                        RoleId = uc.RoleId,
                        AssignedAt = uc.AssignedAt,
                        User = new UserDto
                        {
                            Id = uc.User.Id,
                            Name = $"{uc.User.FirstName} {uc.User.LastName}".Trim(),
                            FirstName = uc.User.FirstName,
                            LastName = uc.User.LastName,
                            Email = uc.User.Email,
                            CI = uc.User.CI,
                            TypeAuth = uc.User.TypeAuth.ToString(),
                            IsActive = uc.User.IsActive,
                            CreatedAt = uc.User.CreatedAt,
                            UpdatedAt = uc.User.UpdatedAt
                        },
                        Role = new RoleDto
                        {
                            Id = uc.Role.Id,
                            Name = uc.Role.Name,
                            Description = uc.Role.Description,
                            CompanyId = uc.Role.CompanyId,
                            Permissions = new List<PermissionDto>() // Se puede expandir si es necesario
                        }
                    })
                    .ToListAsync();

                return Results.Ok(companyUsers);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al obtener usuarios de la compañía", details = ex.Message });
            }
        }

        private static async Task<IResult> GetCompanyUser(Guid companyId, Guid userId, ApplicationDbContext context)
        {
            try
            {
                var companyUser = await context.UserCompanys
                    .Include(uc => uc.User)
                    .Include(uc => uc.Role)
                    .Where(uc => uc.CompanyId == companyId && uc.UserId == userId)
                    .Select(uc => new CompanyUserDto
                    {
                        Id = uc.Id,
                        UserId = uc.UserId,
                        CompanyId = uc.CompanyId,
                        RoleId = uc.RoleId,
                        AssignedAt = uc.AssignedAt,
                        User = new UserDto
                        {
                            Id = uc.User.Id,
                            Name = $"{uc.User.FirstName} {uc.User.LastName}".Trim(),
                            FirstName = uc.User.FirstName,
                            LastName = uc.User.LastName,
                            Email = uc.User.Email,
                            CI = uc.User.CI,
                            TypeAuth = uc.User.TypeAuth.ToString(),
                            IsActive = uc.User.IsActive,
                            CreatedAt = uc.User.CreatedAt,
                            UpdatedAt = uc.User.UpdatedAt
                        },
                        Role = new RoleDto
                        {
                            Id = uc.Role.Id,
                            Name = uc.Role.Name,
                            Description = uc.Role.Description,
                            CompanyId = uc.Role.CompanyId,
                            Permissions = new List<PermissionDto>()
                        }
                    })
                    .FirstOrDefaultAsync();

                if (companyUser == null)
                    return Results.NoContent();

                return Results.Ok(companyUser);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al obtener usuario de la compañía", details = ex.Message });
            }
        }

        private static async Task<IResult> AssignUserToCompany(Guid companyId, AssignUserToCompanyRequest request, ApplicationDbContext context)
        {
            try
            {
                // Verificar que el usuario existe
                var user = await context.Users.FindAsync(request.UserId);
                if (user == null)
                    return Results.BadRequest(new { error = "Usuario no encontrado" });

                // Verificar que el rol existe y pertenece a la compañía
                var role = await context.Roles
                    .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.CompanyId == companyId);
                if (role == null)
                    return Results.BadRequest(new { error = "Rol no encontrado o no pertenece a esta compañía" });

                // Verificar que el usuario no esté ya asignado a esta compañía
                var existingAssignment = await context.UserCompanys
                    .FirstOrDefaultAsync(uc => uc.UserId == request.UserId && uc.CompanyId == companyId);
                if (existingAssignment != null)
                    return Results.BadRequest(new { error = "El usuario ya está asignado a esta compañía" });

                var userCompany = new UserCompany
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    CompanyId = companyId,
                    RoleId = request.RoleId,
                    AssignedAt = DateTime.UtcNow
                };

                context.UserCompanys.Add(userCompany);
                await context.SaveChangesAsync();

                // Retornar el usuario asignado
                var result = await context.UserCompanys
                    .Include(uc => uc.User)
                    .Include(uc => uc.Role)
                    .Where(uc => uc.Id == userCompany.Id)
                    .Select(uc => new CompanyUserDto
                    {
                        Id = uc.Id,
                        UserId = uc.UserId,
                        CompanyId = uc.CompanyId,
                        RoleId = uc.RoleId,
                        AssignedAt = uc.AssignedAt,
                        User = new UserDto
                        {
                            Id = uc.User.Id,
                            Name = $"{uc.User.FirstName} {uc.User.LastName}".Trim(),
                            FirstName = uc.User.FirstName,
                            LastName = uc.User.LastName,
                            Email = uc.User.Email,
                            CI = uc.User.CI,
                            TypeAuth = uc.User.TypeAuth.ToString(),
                            IsActive = uc.User.IsActive,
                            CreatedAt = uc.User.CreatedAt,
                            UpdatedAt = uc.User.UpdatedAt
                        },
                        Role = new RoleDto
                        {
                            Id = uc.Role.Id,
                            Name = uc.Role.Name,
                            Description = uc.Role.Description,
                            CompanyId = uc.Role.CompanyId,
                            Permissions = new List<PermissionDto>()
                        }
                    })
                    .FirstOrDefaultAsync();

                return Results.Created($"/api/companies/{companyId}/users/{request.UserId}", result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al asignar usuario a la compañía", details = ex.Message });
            }
        }

        private static async Task<IResult> ChangeUserRoleInCompany(Guid companyId, Guid userId, ChangeUserRoleRequest request, ApplicationDbContext context)
        {
            try
            {
                // Verificar que el usuario está asignado a la compañía
                var userCompany = await context.UserCompanys
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CompanyId == companyId);
                if (userCompany == null)
                    return Results.BadRequest(new { error = "El usuario no está asignado a esta compañía" });

                // Verificar que el nuevo rol existe y pertenece a la compañía
                var role = await context.Roles
                    .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.CompanyId == companyId);
                if (role == null)
                    return Results.BadRequest(new { error = "Rol no encontrado o no pertenece a esta compañía" });

                userCompany.RoleId = request.RoleId;
                await context.SaveChangesAsync();

                // Retornar el usuario actualizado
                var result = await context.UserCompanys
                    .Include(uc => uc.User)
                    .Include(uc => uc.Role)
                    .Where(uc => uc.Id == userCompany.Id)
                    .Select(uc => new CompanyUserDto
                    {
                        Id = uc.Id,
                        UserId = uc.UserId,
                        CompanyId = uc.CompanyId,
                        RoleId = uc.RoleId,
                        AssignedAt = uc.AssignedAt,
                        User = new UserDto
                        {
                            Id = uc.User.Id,
                            Name = $"{uc.User.FirstName} {uc.User.LastName}".Trim(),
                            FirstName = uc.User.FirstName,
                            LastName = uc.User.LastName,
                            Email = uc.User.Email,
                            CI = uc.User.CI,
                            TypeAuth = uc.User.TypeAuth.ToString(),
                            IsActive = uc.User.IsActive,
                            CreatedAt = uc.User.CreatedAt,
                            UpdatedAt = uc.User.UpdatedAt
                        },
                        Role = new RoleDto
                        {
                            Id = uc.Role.Id,
                            Name = uc.Role.Name,
                            Description = uc.Role.Description,
                            CompanyId = uc.Role.CompanyId,
                            Permissions = new List<PermissionDto>()
                        }
                    })
                    .FirstOrDefaultAsync();

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al cambiar rol del usuario", details = ex.Message });
            }
        }

        private static async Task<IResult> RemoveUserFromCompany(Guid companyId, Guid userId, ApplicationDbContext context)
        {
            try
            {
                var userCompany = await context.UserCompanys
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CompanyId == companyId);
                if (userCompany == null)
                    return Results.BadRequest(new { error = "El usuario no está asignado a esta compañía" });

                context.UserCompanys.Remove(userCompany);
                await context.SaveChangesAsync();

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al remover usuario de la compañía", details = ex.Message });
            }
        }
    }

    // DTOs para los endpoints de usuarios de compañías
    public class CompanyUserDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime AssignedAt { get; set; }
        public UserDto User { get; set; } = new();
        public RoleDto Role { get; set; } = new();
    }

    public class AssignUserToCompanyRequest
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }

    public class ChangeUserRoleRequest
    {
        public Guid RoleId { get; set; }
    }
}
