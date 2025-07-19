using CompanyService.Core.Feature.Commands.Customer;
using CompanyService.Core.Feature.Querys.Customer;
using CompanyService.Core.Models.Customer;
using CompanyService.WebApi.Extensions;
using MediatR;

namespace CompanyService.WebApi.Endpoints
{
    public static class CustomerEndpoints
    {
        public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/companies/{companyId:guid}/customers", CreateCustomer)
                .WithName("CreateCustomer")
                .WithTags("Customers")
                .RequireAuthorization()
                .Accepts<CreateCustomerRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/customers", GetCustomers)
                .WithName("GetCustomers")
                .WithTags("Customers")
                .RequireAuthorization()
                .Produces<List<CustomerDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/customers/{customerId:guid}", GetCustomer)
                .WithName("GetCustomer")
                .WithTags("Customers")
                .RequireAuthorization()
                .Produces<CustomerDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> CreateCustomer(
            Guid companyId,
            CreateCustomerRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateCustomerCommand
            {
                Name = request.Name,
                DocumentNumber = request.DocumentNumber,
                DocumentType = request.DocumentType,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                City = request.City,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var customerId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/customers/{customerId}", new { Id = customerId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetCustomers(
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

            var query = new GetCustomersQuery
            {
                CompanyId = companyId,
                SearchTerm = search,
                Page = page,
                PageSize = pageSize
            };

            var customers = await mediator.Send(query);
            return Results.Ok(customers);
        }

        private static async Task<IResult> GetCustomer(
            Guid companyId,
            Guid customerId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetCustomerByIdQuery
            {
                Id = customerId,
                CompanyId = companyId
            };

            var customer = await mediator.Send(query);
            return customer is null ? Results.NotFound() : Results.Ok(customer);
        }
    }


}
