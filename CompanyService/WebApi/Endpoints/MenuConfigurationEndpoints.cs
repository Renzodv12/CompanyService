using CompanyService.Core.DTOs.Menu;
using CompanyService.Core.Feature.Commands.Menu;
using CompanyService.Core.Feature.Querys.Menu;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CompanyService.WebApi.Endpoints
{
    public static class MenuConfigurationEndpoints
    {
        public static void MapMenuConfigurationEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/companies/{companyId:guid}/menu-configuration")
                .WithTags("Menu Configuration")
                .RequireAuthorization();

            // GET /companies/{companyId}/menu-configuration (cached via Redis)
            group.MapGet("", GetCompanyMenuConfiguration)
                .WithName("GetCompanyMenuConfiguration")
                .WithOpenApi()
                .Produces<CompanyMenuConfigurationDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            // PUT /companies/{companyId}/menu-configuration (invalidates cache)
            group.MapPut("", UpdateCompanyMenuConfiguration)
                .WithName("UpdateCompanyMenuConfiguration")
                .WithOpenApi()
                .Accepts<UpdateMenuConfigurationRequest>("application/json")
                .Produces<bool>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest);
        }

        private static async Task<IResult> GetCompanyMenuConfiguration(
            Guid companyId,
            ISender mediator)
        {
            try
            {
                var query = new GetCompanyMenuConfigurationQuery(companyId);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task<IResult> UpdateCompanyMenuConfiguration(
            Guid companyId,
            UpdateMenuConfigurationRequest request,
            ISender mediator)
        {
            try
            {
                if (request.CompanyId != companyId)
                {
                    return Results.BadRequest(new { message = "Company ID in URL does not match request body" });
                }

                var command = new UpdateCompanyMenuConfigurationCommand(companyId, request.MenuConfigurations);
                var result = await mediator.Send(command);
                
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}

