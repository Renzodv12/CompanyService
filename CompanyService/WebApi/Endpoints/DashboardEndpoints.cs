using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Attributes;
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
                .Produces<CompanyService.Core.Models.Report.DashboardSummaryDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/dashboard/metrics", GetDashboardMetrics)
                .WithName("GetDashboardMetrics")
                .WithTags("Dashboard")
                .RequireAuthorization()
                .Produces<object>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/dashboard/alerts", GetDashboardAlerts)
                .WithName("GetDashboardAlerts")
                .WithTags("Dashboard")
                .RequireAuthorization()
                .Produces<List<CompanyService.Core.Models.Report.AlertDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/dashboard/kpis", GetDashboardKPIs)
                .WithName("GetDashboardKPIs")
                .WithTags("Dashboard")
                .RequireAuthorization()
                .Produces<List<CompanyService.Core.Models.Report.KPIDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> GetDashboard(
            Guid companyId,
            HttpContext httpContext,
            IDashboardService dashboardService,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"dashboard:company:{companyId}";
                var dashboard = await cacheService.GetAsync<CompanyService.Core.Models.Report.DashboardSummaryDto>(
                    cacheKey, 
                    async () => await dashboardService.GetDashboardSummaryAsync(companyId),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(15) });

                return Results.Ok(dashboard);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetDashboardMetrics(
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
                
                var metrics = new
                {
                    Sales = new
                    {
                        TotalThisMonth = dashboard.TotalSalesThisMonth,
                        TotalLastMonth = dashboard.TotalSalesLastMonth,
                        GrowthPercentage = dashboard.SalesGrowthPercentage,
                        CountThisMonth = dashboard.SalesCountThisMonth,
                        CountLastMonth = dashboard.SalesCountLastMonth,
                        AverageAmount = dashboard.AverageSaleAmount
                    },
                    Customers = new
                    {
                        Total = dashboard.TotalCustomers,
                        NewThisMonth = dashboard.NewCustomersThisMonth,
                        ActiveThisMonth = dashboard.ActiveCustomersThisMonth
                    },
                    Products = new
                    {
                        Total = dashboard.TotalProducts,
                        Active = dashboard.ActiveProducts,
                        LowStock = dashboard.LowStockProductsCount,
                        OutOfStock = dashboard.OutOfStockProductsCount,
                        InventoryValue = dashboard.TotalInventoryValue
                    },
                    Financial = new
                    {
                        Revenue = dashboard.TotalRevenue,
                        Expenses = dashboard.TotalExpenses,
                        NetProfit = dashboard.NetProfit,
                        ProfitMargin = dashboard.ProfitMargin
                    },
                    Budgets = new
                    {
                        Total = dashboard.TotalBudgets,
                        Active = dashboard.ActiveBudgets,
                        BudgetedAmount = dashboard.TotalBudgetedAmount,
                        ActualAmount = dashboard.TotalActualAmount,
                        Variance = dashboard.BudgetVariance
                    },
                    CRM = new
                    {
                        TotalLeads = dashboard.TotalLeads,
                        NewLeadsThisMonth = dashboard.NewLeadsThisMonth,
                        ConvertedLeadsThisMonth = dashboard.ConvertedLeadsThisMonth,
                        ConversionRate = dashboard.LeadConversionRate
                    }
                };

                return Results.Ok(metrics);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetDashboardAlerts(
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
                return Results.Ok(dashboard.Alerts);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetDashboardKPIs(
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
                return Results.Ok(dashboard.KPIs);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
}
