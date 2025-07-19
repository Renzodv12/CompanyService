using CompanyService.Core.Feature.Commands.Sale;
using CompanyService.Core.Feature.Querys.Sale;
using CompanyService.Core.Models.Sale;
using CompanyService.WebApi.Extensions;
using MediatR;

namespace CompanyService.WebApi.Endpoints
{
    public static class SupplierEndpoints
    {
        public static IEndpointRouteBuilder MapSupplierEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/companies/{companyId:guid}/suppliers", CreateSupplier)
                .WithName("CreateSupplier")
                .WithTags("Suppliers")
                .RequireAuthorization()
                .Accepts<CreateSupplierRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/suppliers", GetSuppliers)
                .WithName("GetSuppliers")
                .WithTags("Suppliers")
                .RequireAuthorization()
                .Produces<List<SupplierDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> CreateSupplier(
            Guid companyId,
            CreateSupplierRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateSupplierCommand
            {
                Name = request.Name,
                DocumentNumber = request.DocumentNumber,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                City = request.City,
                ContactPerson = request.ContactPerson,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var supplierId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/suppliers/{supplierId}", new { Id = supplierId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetSuppliers(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator,
            string? search = null,
            int page = 1,
            int pageSize = 20)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetSuppliersQuery
            {
                CompanyId = companyId,
                SearchTerm = search,
                Page = page,
                PageSize = pageSize
            };

            var suppliers = await mediator.Send(query);
            return Results.Ok(suppliers);
        }
    }
}
