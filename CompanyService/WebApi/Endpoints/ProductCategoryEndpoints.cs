using CompanyService.Core.Feature.Commands.Product;
using CompanyService.Core.Feature.Querys.Product;
using CompanyService.Core.Models.Product;
using CompanyService.WebApi.Extensions;
using MediatR;

namespace CompanyService.WebApi.Endpoints
{
    public static class ProductCategoryEndpoints
    {
        public static IEndpointRouteBuilder MapProductCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/companies/{companyId:guid}/product-categories", CreateProductCategory)
                .WithName("CreateProductCategory")
                .WithTags("ProductCategories")
                .RequireAuthorization()
                .Accepts<CreateProductCategoryRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/product-categories", GetProductCategories)
                .WithName("GetProductCategories")
                .WithTags("ProductCategories")
                .RequireAuthorization()
                .Produces<List<ProductCategoryDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> CreateProductCategory(
            Guid companyId,
            CreateProductCategoryRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateProductCategoryCommand
            {
                Name = request.Name,
                Description = request.Description,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var categoryId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/product-categories/{categoryId}", new { Id = categoryId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetProductCategories(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator,
            bool? isActive = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetProductCategoriesQuery
            {
                CompanyId = companyId,
                IsActive = isActive
            };

            var categories = await mediator.Send(query);
            return Results.Ok(categories);
        }
    }
}
