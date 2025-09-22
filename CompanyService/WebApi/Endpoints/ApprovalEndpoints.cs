using CompanyService.Core.DTOs;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Enums;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.Feature.Querys.Procurement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CompanyService.WebApi.Endpoints
{
    public static class ApprovalEndpoints
    {
        public static void MapApprovalEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/approval")
                .WithTags("Approval")
                .RequireAuthorization();

            // Approval endpoints
            group.MapGet("/", GetApprovals)
                .WithName("GetApprovals")
                .WithOpenApi();

            group.MapGet("/{id:guid}", GetApproval)
                .WithName("GetApproval")
                .WithOpenApi();

            group.MapPost("/", CreateApproval)
                .WithName("CreateApproval")
                .WithOpenApi();

            group.MapPut("/{id:guid}", UpdateApproval)
                .WithName("UpdateApproval")
                .WithOpenApi();

            group.MapPost("/{id:guid}/process", ProcessApproval)
                .WithName("ProcessApproval")
                .WithOpenApi();

            group.MapDelete("/{id:guid}", DeleteApproval)
                .WithName("DeleteApproval")
                .WithOpenApi();

            group.MapGet("/pending", GetPendingApprovals)
                .WithName("GetPendingApprovals")
                .WithOpenApi();

            // Approval Level endpoints
            group.MapGet("/levels", GetApprovalLevels)
                .WithName("GetApprovalLevels")
                .WithOpenApi();

            group.MapPost("/levels", CreateApprovalLevel)
                .WithName("CreateApprovalLevel")
                .WithOpenApi();

            group.MapPut("/levels/{id:guid}", UpdateApprovalLevel)
                .WithName("UpdateApprovalLevel")
                .WithOpenApi();

            group.MapDelete("/levels/{id:guid}", DeleteApprovalLevel)
                .WithName("DeleteApprovalLevel")
                .WithOpenApi();
        }

        private static async Task<IResult> GetApprovals(
            ClaimsPrincipal user,
            IMediator mediator,
            [FromQuery] ApprovalStatus? status,
            [FromQuery] string? documentType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var companyIdClaim = user.FindFirst("CompanyId")?.Value;
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(companyIdClaim) || string.IsNullOrEmpty(userIdClaim))
                {
                    return Results.Unauthorized();
                }

                var query = new GetApprovalsQuery
                {
                    CompanyId = Guid.Parse(companyIdClaim),
                    Status = status,
                    DocumentType = documentType,
                    Page = page,
                    PageSize = pageSize,
                    UserId = Guid.Parse(userIdClaim)
                };

                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al obtener las aprobaciones: {ex.Message}");
            }
        }

        private static async Task<IResult> GetApproval(
            Guid id,
            ClaimsPrincipal user,
            IMediator mediator)
        {
            try
            {
                var companyIdClaim = user.FindFirst("CompanyId")?.Value;
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(companyIdClaim) || string.IsNullOrEmpty(userIdClaim))
                {
                    return Results.Unauthorized();
                }

                var query = new GetApprovalQuery
                {
                    Id = id,
                    CompanyId = Guid.Parse(companyIdClaim),
                    UserId = Guid.Parse(userIdClaim)
                };

                var result = await mediator.Send(query);
                if (result == null)
                {
                    return Results.NotFound($"Aprobación con ID {id} no encontrada");
                }

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al obtener la aprobación: {ex.Message}");
            }
        }

        private static async Task<IResult> CreateApproval(
            CreateApprovalRequest request,
            ClaimsPrincipal user,
            IMediator mediator)
        {
            try
            {
                var companyIdClaim = user.FindFirst("CompanyId")?.Value;
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(companyIdClaim) || string.IsNullOrEmpty(userIdClaim))
                {
                    return Results.Unauthorized();
                }

                var command = new CreateApprovalCommand
                {
                    DocumentType = request.DocumentType,
                    DocumentId = request.DocumentId,
                    DocumentNumber = request.DocumentNumber,
                    ApprovalLevelId = request.ApprovalLevelId,
                    UserId = request.UserId,
                    DocumentAmount = request.DocumentAmount,
                    Comments = request.Comments,
                    DueDate = request.DueDate,
                    CompanyId = Guid.Parse(companyIdClaim),
                    RequestingUserId = Guid.Parse(userIdClaim)
                };

                var result = await mediator.Send(command);
                return Results.Created($"/api/approval/{result}", new { Id = result });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al crear la aprobación: {ex.Message}");
            }
        }

        private static async Task<IResult> UpdateApproval(
            Guid id,
            UpdateApprovalRequest request,
            ClaimsPrincipal user,
            IMediator mediator)
        {
            try
            {
                var companyIdClaim = user.FindFirst("CompanyId")?.Value;
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(companyIdClaim) || string.IsNullOrEmpty(userIdClaim))
                {
                    return Results.Unauthorized();
                }

                var command = new UpdateApprovalCommand
                {
                    Id = id,
                    Comments = request.Comments,
                    DueDate = request.DueDate,
                    CompanyId = Guid.Parse(companyIdClaim),
                    UserId = Guid.Parse(userIdClaim)
                };

                var result = await mediator.Send(command);
                if (!result)
                {
                    return Results.NotFound($"Aprobación con ID {id} no encontrada");
                }

                return Results.Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al actualizar la aprobación: {ex.Message}");
            }
        }

        private static async Task<IResult> ProcessApproval(
            Guid id,
            ProcessApprovalRequest request,
            ClaimsPrincipal user,
            IMediator mediator)
        {
            try
            {
                var companyIdClaim = user.FindFirst("CompanyId")?.Value;
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(companyIdClaim) || string.IsNullOrEmpty(userIdClaim))
                {
                    return Results.Unauthorized();
                }

                var command = new ProcessApprovalCommand
                {
                    Id = id,
                    Action = request.Action,
                    Comments = request.Comments,
                    DelegateToUserId = request.DelegateToUserId,
                    CompanyId = Guid.Parse(companyIdClaim),
                    UserId = Guid.Parse(userIdClaim)
                };

                var result = await mediator.Send(command);
                if (!result)
                {
                    return Results.NotFound($"Aprobación con ID {id} no encontrada");
                }

                return Results.Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al procesar la aprobación: {ex.Message}");
            }
        }

        private static async Task<IResult> DeleteApproval(
            Guid id,
            ClaimsPrincipal user,
            IMediator mediator)
        {
            try
            {
                var companyIdClaim = user.FindFirst("CompanyId")?.Value;
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(companyIdClaim) || string.IsNullOrEmpty(userIdClaim))
                {
                    return Results.Unauthorized();
                }

                var command = new DeleteApprovalCommand
                {
                    Id = id,
                    CompanyId = Guid.Parse(companyIdClaim),
                    UserId = Guid.Parse(userIdClaim)
                };

                var result = await mediator.Send(command);
                if (!result)
                {
                    return Results.NotFound($"Aprobación con ID {id} no encontrada");
                }

                return Results.Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al eliminar la aprobación: {ex.Message}");
            }
        }

        private static async Task<IResult> GetPendingApprovals(
            ClaimsPrincipal user,
            IMediator mediator,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var companyIdClaim = user.FindFirst("CompanyId")?.Value;
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(companyIdClaim) || string.IsNullOrEmpty(userIdClaim))
                {
                    return Results.Unauthorized();
                }

                var query = new GetPendingApprovalsQuery
                {
                    CompanyId = Guid.Parse(companyIdClaim),
                    Page = page,
                    PageSize = pageSize,
                    UserId = Guid.Parse(userIdClaim)
                };

                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al obtener las aprobaciones pendientes: {ex.Message}");
            }
        }

        private static async Task<IResult> GetApprovalLevels(
            ClaimsPrincipal user,
            IMediator mediator,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var companyIdClaim = user.FindFirst("CompanyId")?.Value;
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(companyIdClaim) || string.IsNullOrEmpty(userIdClaim))
                {
                    return Results.Unauthorized();
                }

                var query = new GetApprovalLevelsQuery
                {
                    CompanyId = Guid.Parse(companyIdClaim),
                    Page = page,
                    PageSize = pageSize,
                    UserId = Guid.Parse(userIdClaim)
                };

                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al obtener los niveles de aprobación: {ex.Message}");
            }
        }

        private static async Task<IResult> CreateApprovalLevel(
            CreateApprovalLevelRequest request,
            ClaimsPrincipal user,
            IMediator mediator)
        {
            try
            {
                var companyIdClaim = user.FindFirst("CompanyId")?.Value;
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(companyIdClaim) || string.IsNullOrEmpty(userIdClaim))
                {
                    return Results.Unauthorized();
                }

                var command = new CreateApprovalLevelCommand
                {
                    Name = request.Name,
                    Description = request.Description,
                    Level = request.Level,
                    MinAmount = request.MinAmount,
                    MaxAmount = request.MaxAmount,
                    RequiresAllApprovers = request.RequiresAllApprovers,
                    IsActive = request.IsActive,
                    CompanyId = Guid.Parse(companyIdClaim),
                    UserId = Guid.Parse(userIdClaim)
                };

                var result = await mediator.Send(command);
                return Results.Created($"/api/approval/levels/{result}", new { Id = result });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al crear el nivel de aprobación: {ex.Message}");
            }
        }

        private static async Task<IResult> UpdateApprovalLevel(
            Guid id,
            UpdateApprovalLevelRequest request,
            ClaimsPrincipal user,
            IMediator mediator)
        {
            try
            {
                var companyIdClaim = user.FindFirst("CompanyId")?.Value;
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(companyIdClaim) || string.IsNullOrEmpty(userIdClaim))
                {
                    return Results.Unauthorized();
                }

                var command = new UpdateApprovalLevelCommand
                {
                    Id = id,
                    CompanyId = Guid.Parse(companyIdClaim),
                    Request = request
                };

                var result = await mediator.Send(command);
                if (result == null)
                {
                    return Results.NotFound($"Nivel de aprobación con ID {id} no encontrado");
                }

                return Results.Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al actualizar el nivel de aprobación: {ex.Message}");
            }
        }

        private static async Task<IResult> DeleteApprovalLevel(
            Guid id,
            ClaimsPrincipal user,
            IMediator mediator)
        {
            try
            {
                var companyIdClaim = user.FindFirst("CompanyId")?.Value;
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(companyIdClaim) || string.IsNullOrEmpty(userIdClaim))
                {
                    return Results.Unauthorized();
                }

                var command = new DeleteApprovalLevelCommand
                {
                    Id = id,
                    CompanyId = Guid.Parse(companyIdClaim),
                    UserId = Guid.Parse(userIdClaim)
                };

                var result = await mediator.Send(command);
                if (!result)
                {
                    return Results.NotFound($"Nivel de aprobación con ID {id} no encontrado");
                }

                return Results.Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al eliminar el nivel de aprobación: {ex.Message}");
            }
        }
    }
}