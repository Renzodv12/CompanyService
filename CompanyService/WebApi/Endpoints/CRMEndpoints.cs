using CompanyService.Core.Feature.Commands.CRM;
using CompanyService.Core.Feature.Querys.CRM;
using CompanyService.Core.Models.CRM;
using CompanyService.Core.DTOs.CRM;
using CompanyService.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace CompanyService.WebApi.Endpoints
{
    public static class CRMEndpoints
    {
        public static IEndpointRouteBuilder MapCRMEndpoints(this IEndpointRouteBuilder app)
        {
            // Lead endpoints
            app.MapGet("/companies/{companyId:guid}/crm/leads", GetLeads)
                .WithName("GetLeads")
                .WithTags("CRM")
                .RequireAuthorization()
                .Produces<object>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/crm/leads/{id:guid}", GetLead)
                .WithName("GetLead")
                .WithTags("CRM")
                .RequireAuthorization()
                .Produces<object>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPost("/companies/{companyId:guid}/crm/leads", CreateLead)
                .WithName("CreateLead")
                .WithTags("CRM")
                .RequireAuthorization()
                .Accepts<CreateLeadRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/crm/leads/{id:guid}", UpdateLead)
                .WithName("UpdateLead")
                .WithTags("CRM")
                .RequireAuthorization()
                .Accepts<UpdateLeadRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapDelete("/companies/{companyId:guid}/crm/leads/{id:guid}", DeleteLead)
                .WithName("DeleteLead")
                .WithTags("CRM")
                .RequireAuthorization()
                .Produces(StatusCodes.Status204NoContent)
                .WithOpenApi();

            app.MapPost("/companies/{companyId:guid}/crm/leads/{id:guid}/convert", ConvertLead)
                .WithName("ConvertLead")
                .WithTags("CRM")
                .RequireAuthorization()
                .Accepts<ConvertLeadRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            // Opportunity endpoints
            app.MapGet("/companies/{companyId:guid}/crm/opportunities", GetOpportunities)
                .WithName("GetOpportunities")
                .WithTags("CRM")
                .RequireAuthorization()
                .Produces<object>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/crm/opportunities/{id:guid}", GetOpportunity)
                .WithName("GetOpportunity")
                .WithTags("CRM")
                .RequireAuthorization()
                .Produces<object>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPost("/companies/{companyId:guid}/crm/opportunities", CreateOpportunity)
                .WithName("CreateOpportunity")
                .WithTags("CRM")
                .RequireAuthorization()
                .Accepts<CreateOpportunityRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/crm/opportunities/{id:guid}", UpdateOpportunity)
                .WithName("UpdateOpportunity")
                .WithTags("CRM")
                .RequireAuthorization()
                .Accepts<UpdateOpportunityRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapDelete("/companies/{companyId:guid}/crm/opportunities/{id:guid}", DeleteOpportunity)
                .WithName("DeleteOpportunity")
                .WithTags("CRM")
                .RequireAuthorization()
                .Produces(StatusCodes.Status204NoContent)
                .WithOpenApi();

            // Campaign endpoints
            app.MapGet("/companies/{companyId:guid}/crm/campaigns", GetCampaigns)
                .WithName("GetCampaigns")
                .WithTags("CRM")
                .RequireAuthorization()
                .Produces<object>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPost("/companies/{companyId:guid}/crm/campaigns", CreateCampaign)
                .WithName("CreateCampaign")
                .WithTags("CRM")
                .RequireAuthorization()
                .Accepts<CreateCampaignRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/crm/campaigns/{id:guid}", UpdateCampaign)
                .WithName("UpdateCampaign")
                .WithTags("CRM")
                .RequireAuthorization()
                .Accepts<UpdateCampaignRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapDelete("/companies/{companyId:guid}/crm/campaigns/{id:guid}", DeleteCampaign)
                .WithName("DeleteCampaign")
                .WithTags("CRM")
                .RequireAuthorization()
                .Produces(StatusCodes.Status204NoContent)
                .WithOpenApi();

            // Dashboard endpoint
            app.MapGet("/companies/{companyId:guid}/crm/dashboard", GetCRMDashboard)
                .WithName("GetCRMDashboard")
                .WithTags("CRM")
                .RequireAuthorization()
                .Produces<object>(StatusCodes.Status200OK)
                .WithOpenApi();

            return app;
        }

        // Lead methods
        private static async Task<IResult> GetLeads(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator,
            int page = 1,
            int pageSize = 20)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var query = new GetLeadsQuery
            {
                CompanyId = companyId,
                Page = page,
                PageSize = pageSize,
                UserId = userId
            };

            try
            {
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetLead(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var query = new GetLeadQuery
            {
                Id = id,
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateLead(
            Guid companyId,
            CreateLeadRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var command = new CreateLeadCommand
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Company = request.CompanyName,
                Position = request.JobTitle,
                Source = request.Source.ToString(),
                Status = request.Status.ToString(),
                Notes = request.Notes,
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                var leadId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/crm/leads/{leadId}", new { Id = leadId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateLead(
            Guid companyId,
            Guid id,
            UpdateLeadRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var command = new UpdateLeadCommand
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Company = request.CompanyName,
                Position = request.JobTitle,
                Source = request.Source.ToString(),
                Status = request.Status.ToString(),
                Notes = request.Notes,
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeleteLead(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var command = new DeleteLeadCommand
            {
                Id = id,
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                await mediator.Send(command);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> ConvertLead(
            Guid companyId,
            Guid id,
            ConvertLeadRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var command = new ConvertLeadCommand
            {
                LeadId = id,
                OpportunityName = request.OpportunityName,
                OpportunityDescription = request.OpportunityDescription,
                EstimatedValue = request.EstimatedValue,
                Probability = request.Probability,
                ExpectedCloseDate = request.ExpectedCloseDate,
                AssignedToUserId = request.AssignedToUserId,
                Notes = request.Notes,
                CreateCustomer = request.CreateCustomer,
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                var opportunityId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/crm/opportunities/{opportunityId}", new { Id = opportunityId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        // Opportunity methods
        private static async Task<IResult> GetOpportunities(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator,
            int page = 1,
            int pageSize = 20)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var query = new GetOpportunitiesQuery
            {
                CompanyId = companyId,
                Page = page,
                PageSize = pageSize,
                UserId = userId
            };

            try
            {
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetOpportunity(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var query = new GetOpportunityQuery
            {
                Id = id,
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateOpportunity(
            Guid companyId,
            CreateOpportunityRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var command = new CreateOpportunityCommand
            {
                Name = request.Name,
                Description = request.Description,
                EstimatedValue = (decimal?)request.Value,
                ExpectedCloseDate = request.ExpectedCloseDate,
                Stage = request.Stage.ToString(),
                Probability = (decimal?)request.Probability,
                LeadId = request.LeadId,
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                var opportunityId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/crm/opportunities/{opportunityId}", new { Id = opportunityId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateOpportunity(
            Guid companyId,
            Guid id,
            UpdateOpportunityRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var command = new UpdateOpportunityCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                EstimatedValue = (decimal?)request.Value,
                ExpectedCloseDate = request.ExpectedCloseDate,
                Stage = request.Stage.ToString(),
                Probability = (decimal?)request.Probability,
                LeadId = request.LeadId,
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeleteOpportunity(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var command = new DeleteOpportunityCommand
            {
                Id = id,
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                await mediator.Send(command);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        // Campaign methods
        private static async Task<IResult> GetCampaigns(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var query = new GetCampaignsQuery
            {
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateCampaign(
            Guid companyId,
            CreateCampaignRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var command = new CreateCampaignCommand
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Budget = (decimal?)request.Budget,
                Type = request.Type.ToString(),
                Status = request.Status.ToString(),
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                var campaignId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/crm/campaigns/{campaignId}", new { Id = campaignId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateCampaign(
            Guid companyId,
            Guid id,
            UpdateCampaignRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var command = new UpdateCampaignCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Budget = (decimal?)request.Budget,
                Type = request.Type.ToString(),
                Status = request.Status.ToString(),
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeleteCampaign(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var command = new DeleteCampaignCommand
            {
                Id = id,
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                await mediator.Send(command);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        // Dashboard method
        private static async Task<IResult> GetCRMDashboard(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            if (!Guid.TryParse(claims.UserId, out var userId))
                return Results.Unauthorized();

            var query = new GetCRMDashboardQuery
            {
                CompanyId = companyId,
                UserId = userId
            };

            try
            {
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
}