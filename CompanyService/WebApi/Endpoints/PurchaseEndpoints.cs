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

            app.MapGet("/companies/{companyId:guid}/purchases/{id:guid}", GetPurchaseById)
                .WithName("GetPurchaseById")
                .WithTags("Purchases")
                .RequireAuthorization()
                .Produces<PurchaseDetailDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/purchases/{id:guid}", UpdatePurchase)
                .WithName("UpdatePurchase")
                .WithTags("Purchases")
                .RequireAuthorization()
                .Accepts<UpdatePurchaseRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapDelete("/companies/{companyId:guid}/purchases/{id:guid}", DeletePurchase)
                .WithName("DeletePurchase")
                .WithTags("Purchases")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
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

        private static async Task<IResult> GetPurchaseById(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetPurchaseByIdQuery
            {
                Id = id,
                CompanyId = companyId
            };

            try
            {
                var purchase = await mediator.Send(query);
                if (purchase == null)
                    return Results.NoContent();

                return Results.Ok(purchase);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdatePurchase(
            Guid companyId,
            Guid id,
            UpdatePurchaseRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdatePurchaseCommand
            {
                Id = id,
                SupplierId = request.SupplierId,
                DeliveryDate = request.DeliveryDate,
                InvoiceNumber = request.InvoiceNumber,
                Notes = request.Notes ?? string.Empty,
                Items = request.Items,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var result = await mediator.Send(command);
                return Results.Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeletePurchase(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new DeletePurchaseCommand
            {
                Id = id,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var result = await mediator.Send(command);
                return Results.Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
}
