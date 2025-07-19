using CompanyService.Core.Interfaces;
using CompanyService.WebApi.Extensions;

namespace CompanyService.WebApi.Endpoints
{
    public static class DashboardEndpoints
    {
        public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/companies/{companyId:guid}/dashboard", GetDashboard)
                .WithName("GetDashboard")
                .WithTags("Dashboard")
                .RequireAuthorization()
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> GetDashboard(
            Guid companyId,
            HttpContext httpContext,
            IDashboardService dashboardService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var dashboard = await dashboardService.GetDashboardSummaryAsync(companyId);
                return Results.Ok(dashboard);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
}
