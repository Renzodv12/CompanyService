using CompanyService.Core.Feature.Commands.Sale;
using CompanyService.Core.Feature.Querys.Sale;
using CompanyService.Core.Models.Sale;
using CompanyService.WebApi.Extensions;
using MediatR;

namespace CompanyService.WebApi.Endpoints
{
    public static class PurchaseEndpoints
    {
        public static IEndpointRouteBuilder MapPurchaseEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/companies/{companyId:guid}/purchases", CreatePurchase)
                .WithName("CreatePurchase")
                .WithTags("Purchases")
                .RequireAuthorization()
                .Accepts<CreatePurchaseRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/purchases", GetPurchases)
                .WithName("GetPurchases")
                .WithTags("Purchases")
                .RequireAuthorization()
                .Produces<List<PurchaseDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> CreatePurchase(
            Guid companyId,
            CreatePurchaseRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreatePurchaseCommand
            {
                SupplierId = request.SupplierId,
                DeliveryDate = request.DeliveryDate,
                InvoiceNumber = request.InvoiceNumber,
                Notes = request.Notes,
                Items = request.Items.Select(i => new PurchaseDetailItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitCost = i.UnitCost
                }).ToList(),
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var purchaseId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/purchases/{purchaseId}", new { Id = purchaseId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetPurchases(
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

            var query = new GetPurchasesQuery
            {
                CompanyId = companyId,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize
            };

            var purchases = await mediator.Send(query);
            return Results.Ok(purchases);
        }
    }
}
