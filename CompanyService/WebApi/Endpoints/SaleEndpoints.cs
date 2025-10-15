using CompanyService.Core.Feature.Commands.Sale;
using CompanyService.Core.Feature.Querys.Sale;
using CompanyService.Core.Models.Sale;
using CompanyService.WebApi.Extensions;
using MediatR;
using FluentValidation;

namespace CompanyService.WebApi.Endpoints
{
    public static class SaleEndpoints
    {
        public static IEndpointRouteBuilder MapSaleEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/companies/{companyId:guid}/sales", CreateSale)
                .WithName("CreateSale")
                .WithTags("Sales")
                .RequireAuthorization()
                .Accepts<CreateSaleRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/sales", GetSales)
                .WithName("GetSales")
                .WithTags("Sales")
                .RequireAuthorization()
                .Produces<List<SaleDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/sales/{saleId:guid}", GetSale)
                .WithName("GetSale")
                .WithTags("Sales")
                .RequireAuthorization()
                .Produces<SaleDetailDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> CreateSale(
            Guid companyId,
            CreateSaleRequest request,
            IValidator<CreateSaleRequest> validator,
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

            var command = new CreateSaleCommand
            {
                CustomerId = request.CustomerId,
                PaymentMethod = request.PaymentMethod,
                Notes = request.Notes,
                DiscountAmount = request.DiscountAmount,
                GenerateElectronicInvoice = request.GenerateElectronicInvoice,
                Items = request.Items,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var saleId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/sales/{saleId}", new { Id = saleId });
            }
            catch (ApplicationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem("Error interno del servidor", statusCode: 500);
            }
        }

        private static async Task<IResult> GetSales(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 20)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetSalesQuery
            {
                CompanyId = companyId,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize
            };

            var sales = await mediator.Send(query);
            return Results.Ok(sales);
        }

        private static async Task<IResult> GetSale(
            Guid companyId,
            Guid saleId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetSaleByIdQuery
            {
                Id = saleId,
                CompanyId = companyId
            };

            var sale = await mediator.Send(query);
            return sale is null ? Results.NoContent() : Results.Ok(sale);
        }
    }


}
