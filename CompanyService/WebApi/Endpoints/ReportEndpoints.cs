using CompanyService.Core.Interfaces;
using CompanyService.WebApi.Extensions;

namespace CompanyService.WebApi.Endpoints
{
    public static class ReportEndpoints
    {
        public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/companies/{companyId:guid}/reports/sales", GetSalesReport)
                .WithName("GetSalesReport")
                .WithTags("Reports")
                .RequireAuthorization()
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/reports/inventory", GetInventoryReport)
                .WithName("GetInventoryReport")
                .WithTags("Reports")
                .RequireAuthorization()
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> GetSalesReport(
            Guid companyId,
            HttpContext httpContext,
            IReportService reportService,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var from = fromDate ?? DateTime.UtcNow.AddDays(-30);
            var to = toDate ?? DateTime.UtcNow;

            try
            {
                var report = await reportService.GenerateSalesReportAsync(companyId, from, to);
                return Results.Ok(report);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetInventoryReport(
            Guid companyId,
            HttpContext httpContext,
            IReportService reportService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var report = await reportService.GenerateInventoryReportAsync(companyId);
                return Results.Ok(report);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
}
