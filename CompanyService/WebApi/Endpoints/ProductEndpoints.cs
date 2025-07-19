using CompanyService.Core.Feature.Commands.Product;
using CompanyService.Core.Feature.Querys.Product;
using CompanyService.Core.Models.Product;
using CompanyService.WebApi.Extensions;
using MediatR;

namespace CompanyService.WebApi.Endpoints
{
    public static class ProductEndpoints
    {
        public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/companies/{companyId:guid}/products", CreateProduct)
                .WithName("CreateProduct")
                .WithTags("Products")
                .RequireAuthorization()
                .Accepts<CreateProductRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/products", GetProducts)
                .WithName("GetProducts")
                .WithTags("Products")
                .RequireAuthorization()
                .Produces<List<ProductDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/products/{productId:guid}", GetProduct)
                .WithName("GetProduct")
                .WithTags("Products")
                .RequireAuthorization()
                .Produces<ProductDetailDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/products/low-stock", GetLowStockProducts)
                .WithName("GetLowStockProducts")
                .WithTags("Products")
                .RequireAuthorization()
                .Produces<List<ProductDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/products/{productId:guid}/stock", UpdateStock)
                .WithName("UpdateStock")
                .WithTags("Products")
                .RequireAuthorization()
                .Accepts<UpdateStockRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/products/{productId:guid}", UpdateProduct)
                .WithName("UpdateProduct")
                .WithTags("Products")
                .RequireAuthorization()
                .Accepts<UpdateProductRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> CreateProduct(
            Guid companyId,
            CreateProductRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateProductCommand
            {
                Name = request.Name,
                Description = request.Description,
                SKU = request.SKU,
                Barcode = request.Barcode,
                Price = request.Price,
                Cost = request.Cost,
                Stock = request.Stock,
                MinStock = request.MinStock,
                MaxStock = request.MaxStock,
                Unit = request.Unit,
                Weight = request.Weight,
                CategoryId = request.CategoryId,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var productId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/products/{productId}", new { Id = productId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetProducts(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator,
            string? search = null,
            Guid? categoryId = null,
            bool? isActive = null,
            int page = 1,
            int pageSize = 20)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetProductsQuery
            {
                CompanyId = companyId,
                SearchTerm = search,
                CategoryId = categoryId,
                IsActive = isActive,
                Page = page,
                PageSize = pageSize
            };

            var products = await mediator.Send(query);
            return Results.Ok(products);
        }

        private static async Task<IResult> GetProduct(
            Guid companyId,
            Guid productId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetProductByIdQuery
            {
                Id = productId,
                CompanyId = companyId
            };

            var product = await mediator.Send(query);
            return product is null ? Results.NotFound() : Results.Ok(product);
        }

        private static async Task<IResult> GetLowStockProducts(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetLowStockProductsQuery
            {
                CompanyId = companyId
            };

            var products = await mediator.Send(query);
            return Results.Ok(products);
        }

        private static async Task<IResult> UpdateStock(
            Guid companyId,
            Guid productId,
            UpdateStockRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdateStockCommand
            {
                ProductId = productId,
                Quantity = request.Quantity,
                MovementType = request.MovementType,
                Reason = request.Reason,
                Reference = request.Reference,
                CompanyId = companyId,
                UserId = claims.UserId
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

        private static async Task<IResult> UpdateProduct(
    Guid companyId,
    Guid productId,
    UpdateProductRequest request,
    HttpContext httpContext,
    ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Cost = request.Cost,
                MinStock = request.MinStock,
                MaxStock = request.MaxStock,
                Unit = request.Unit,
                Weight = request.Weight,
                CategoryId = request.CategoryId,
                IsActive = request.IsActive,
                CompanyId = companyId,
                UserId = claims.UserId
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
    }

  
}
