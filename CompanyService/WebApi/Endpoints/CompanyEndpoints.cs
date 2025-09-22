

using CompanyService.Core.Feature.Commands.Company;
using CompanyService.Core.Feature.Querys.Company;
using CompanyService.Core.Models.Company;
using CompanyService.WebApi.Extensions;
using FluentValidation;
using MediatR;

namespace CompanyService.WebApi.Endpoints
{
    public static class CompanyEndpoints
    {
        public static IEndpointRouteBuilder MapCompanyEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/companies",  Company)
                .WithName("CreateCompany")
                .WithTags("Companies")
                .RequireAuthorization()
                .Accepts<CreateCompanyRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();

            app.MapGet("/companies", Companies  )
                .WithName("GetUserCompanies")
                .WithTags("Companies")
                .RequireAuthorization()
                .Produces<List<CompanyService.Core.Entities.Company>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}", CompaniesByGuid)
                .WithName("GetCompanyDetailWithPermissions")
                .WithTags("Companies")
                .RequireAuthorization()
                .Produces<CompanyWithPermissionsDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();


            return app;
        }
        private static async Task<IResult> Company(
                CreateCompanyRequest request,
                IValidator<CreateCompanyRequest> validator,
                HttpContext httpContext,
                ISender mediator)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateCompanyCommand
            {
                Name = request.Name,
                RUC = request.RUC,
                UserId = claims.UserId
            };

            try
            {
                var companyId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}", new { Id = companyId });
            }
            catch (ApplicationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    
    
        private static async Task<IResult> Companies(
                        HttpContext httpContext,
                        ISender mediator)
        {
            var (jti, userId, _, _) = httpContext.ExtractTokenClaims();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var companies = await mediator.Send(new GetCompaniesByUserIdQuery
            {
                UserId = Guid.Parse(userId)
            });

            return Results.Ok(companies);
        }

        private static async Task<IResult> CompaniesByGuid(
                Guid companyId,
                HttpContext httpContext,
                ISender mediator)
        {
            var (_, userId, _, _) = httpContext.ExtractTokenClaims();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var companyDetail = await mediator.Send(new GetCompanyDetailQuery
            {
                UserId = Guid.Parse(userId),
                CompanyId = companyId
            });

            return companyDetail is null
                ? Results.NoContent()
                : Results.Ok(companyDetail);

        }
    }
}
