using CompanyService.Core.Entities;
using CompanyService.Core.DTOs;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;
using System.Security.Cryptography;
using System.Text;

namespace CompanyService.WebApi.Endpoints
{
    public static class UserEndpoints
    {
        public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/users/email/{email}", GetUserByEmail)
                .WithName("GetUserByEmail")
                .WithTags("Users")
                .WithOpenApi()
                .RequireAuthorization()
                .Produces<UserDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            app.MapGet("/api/users/{id:guid}", GetUserById)
               .WithName("GetUserById")
               .WithTags("Users")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces<UserDto>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound);

            app.MapPost("/api/users", CreateUser)
               .WithName("CreateUser")
               .WithTags("Users")
               .WithOpenApi()
               .RequireAuthorization()
               .Accepts<CreateUserRequest>("application/json")
               .Produces<UserDto>(StatusCodes.Status201Created)
               .Produces(StatusCodes.Status400BadRequest);

            app.MapPut("/api/users/{id:guid}", UpdateUser)
               .WithName("UpdateUser")
               .WithTags("Users")
               .WithOpenApi()
               .RequireAuthorization()
               .Accepts<UpdateUserRequest>("application/json")
               .Produces<UserDto>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound)
               .Produces(StatusCodes.Status400BadRequest);

            app.MapDelete("/api/users/{id:guid}", DeleteUser)
               .WithName("DeleteUser")
               .WithTags("Users")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status404NotFound);

            app.MapGet("/api/users/{id:guid}/roles", GetUserRoles)
               .WithName("GetUserRoles")
               .WithTags("Users")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces<IEnumerable<RoleDto>>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound);

            app.MapPost("/api/users/{userId:guid}/roles/{roleId:guid}", AssignRoleToUser)
               .WithName("AssignRoleToUser")
               .WithTags("Users")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound)
               .Produces(StatusCodes.Status400BadRequest);

            app.MapDelete("/api/users/{userId:guid}/roles/{roleId:guid}", RemoveRoleFromUser)
               .WithName("RemoveRoleFromUser")
               .WithTags("Users")
               .WithOpenApi()
               .RequireAuthorization()
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status404NotFound);

            app.MapPut("/api/users/{id:guid}/password", ChangeUserPassword)
               .WithName("ChangeUserPassword")
               .WithTags("Users")
               .WithOpenApi()
               .RequireAuthorization()
               .Accepts<ChangePasswordRequest>("application/json")
               .Produces(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound)
               .Produces(StatusCodes.Status400BadRequest);

            return app;
        }



        private static async Task<IResult> GetUserById(Guid id, ApplicationDbContext context)
        {
            try
            {
                var user = await context.Users
                    .Where(u => u.Id == id)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Username = u.Username,
                        Email = u.Email,
                        CI = u.CI,
                        TypeAuth = u.TypeAuth.ToString(),
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });

                return Results.Ok(user);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al obtener usuario", details = ex.Message });
            }
        }

        private static async Task<IResult> GetUserByEmail(string email, ApplicationDbContext context)
        {
            try
            {
                // Validar formato de email
                if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                {
                    return Results.BadRequest(new { error = "Formato de email inválido" });
                }

                var user = await context.Users
                    .Where(u => u.Email == email)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Username = u.Username,
                        Email = u.Email,
                        CI = u.CI,
                        TypeAuth = u.TypeAuth.ToString(),
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });

                return Results.Ok(user);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al obtener usuario por email", details = ex.Message });
            }
        }

        private static async Task<IResult> CreateUser(CreateUserRequest request, ApplicationDbContext context)
        {
            try
            {
                // Validar que no exista un usuario con el mismo email
                var existingUser = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (existingUser != null)
                    return Results.BadRequest(new { error = "Ya existe un usuario con ese email" });

                // Generar salt y hash de la contraseña
                var salt = GenerateSalt();
                var hashedPassword = HashPassword(request.Password, salt);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Username = request.Username,
                    Email = request.Email,
                    Password = hashedPassword,
                    Salt = salt,
                    CI = request.CI,
                    TypeAuth = Enum.Parse<TypeAuth>(request.TypeAuth),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Email = user.Email,
                    CI = user.CI,
                    TypeAuth = user.TypeAuth.ToString(),
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return Results.Created($"/api/users/{user.Id}", userDto);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al crear usuario", details = ex.Message });
            }
        }

        private static async Task<IResult> UpdateUser(Guid id, UpdateUserRequest request, ApplicationDbContext context)
        {
            try
            {
                var user = await context.Users.FindAsync(id);
                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });

                // Validar que no exista otro usuario con el mismo email
                var existingUser = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.Id != id);

                if (existingUser != null)
                    return Results.BadRequest(new { error = "Ya existe otro usuario con ese email" });

                user.Name = request.Name;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Username = request.Username;
                user.Email = request.Email;
                user.CI = request.CI;
                user.TypeAuth = Enum.Parse<TypeAuth>(request.TypeAuth);
                user.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Email = user.Email,
                    CI = user.CI,
                    TypeAuth = user.TypeAuth.ToString(),
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return Results.Ok(userDto);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al actualizar usuario", details = ex.Message });
            }
        }

        private static async Task<IResult> DeleteUser(Guid id, ApplicationDbContext context)
        {
            try
            {
                var user = await context.Users
                    .Include(u => u.UserCompanies)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });

                // Verificar si el usuario tiene empresas asignadas
                if (user.UserCompanies.Any())
                    return Results.BadRequest(new { error = "No se puede eliminar el usuario porque tiene empresas asignadas" });

                context.Users.Remove(user);
                await context.SaveChangesAsync();

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al eliminar usuario", details = ex.Message });
            }
        }

        private static async Task<IResult> GetUserRoles(Guid id, ApplicationDbContext context)
        {
            try
            {
                var user = await context.Users
                    .Include(u => u.UserCompanies)
                    .ThenInclude(uc => uc.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });

                var roles = user.UserCompanies.Select(uc => new RoleDto
                {
                    Id = uc.Role.Id,
                    Name = uc.Role.Name,
                    Description = uc.Role.Description,
                    CompanyId = uc.Role.CompanyId,
                    Permissions = new List<PermissionDto>()
                }).ToList();

                return Results.Ok(roles);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al obtener roles del usuario", details = ex.Message });
            }
        }

        private static async Task<IResult> AssignRoleToUser(Guid userId, Guid roleId, ApplicationDbContext context)
        {
            try
            {
                var user = await context.Users.FindAsync(userId);
                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });

                var role = await context.Roles.FindAsync(roleId);
                if (role == null)
                    return Results.NotFound(new { error = "Rol no encontrado" });

                // Verificar si ya existe la asignación
                var existingAssignment = await context.UserCompanys
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.RoleId == roleId);

                if (existingAssignment != null)
                    return Results.BadRequest(new { error = "El usuario ya tiene asignado este rol" });

                var userCompany = new UserCompany
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CompanyId = role.CompanyId,
                    RoleId = roleId
                };

                context.UserCompanys.Add(userCompany);
                await context.SaveChangesAsync();

                return Results.Ok(new { message = "Rol asignado exitosamente al usuario" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al asignar rol al usuario", details = ex.Message });
            }
        }

        private static async Task<IResult> RemoveRoleFromUser(Guid userId, Guid roleId, ApplicationDbContext context)
        {
            try
            {
                var userCompany = await context.UserCompanys
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.RoleId == roleId);

                if (userCompany == null)
                    return Results.NotFound(new { error = "Asignación de rol no encontrada" });

                context.UserCompanys.Remove(userCompany);
                await context.SaveChangesAsync();

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al remover rol del usuario", details = ex.Message });
            }
        }

        private static async Task<IResult> ChangeUserPassword(Guid id, ChangePasswordRequest request, ApplicationDbContext context)
        {
            try
            {
                var user = await context.Users.FindAsync(id);
                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });

                // Verificar contraseña actual
                var currentPasswordHash = HashPassword(request.CurrentPassword, user.Salt);
                if (currentPasswordHash != user.Password)
                    return Results.BadRequest(new { error = "Contraseña actual incorrecta" });

                // Generar nuevo salt y hash para la nueva contraseña
                var newSalt = GenerateSalt();
                var newPasswordHash = HashPassword(request.NewPassword, newSalt);

                user.Password = newPasswordHash;
                user.Salt = newSalt;
                user.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();

                return Results.Ok(new { message = "Contraseña actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = "Error al cambiar contraseña", details = ex.Message });
            }
        }

        private static string GenerateSalt()
        {
            var saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private static string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];
            
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }

    // DTOs para los endpoints de usuarios
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string CI { get; set; }
        public string TypeAuth { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CI { get; set; }
        public string TypeAuth { get; set; }
    }

    public class UpdateUserRequest
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string CI { get; set; }
        public string TypeAuth { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}