using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CompanyService.WebApi.Endpoints
{
    public static class CashFlowEndpoints
    {
        public static void MapCashFlowEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/cashflow")
                .WithTags("CashFlow")
                .RequireAuthorization();

            // Basic CRUD operations
            group.MapPost("/", CreateCashFlow)
                .WithName("CreateCashFlow")
                .WithOpenApi();

            group.MapGet("/{id:guid}", GetCashFlowById)
                .WithName("GetCashFlowById")
                .WithOpenApi();

            group.MapGet("/", GetCashFlowsByCompany)
                .WithName("GetCashFlowsByCompany")
                .WithOpenApi();

            group.MapPut("/{id:guid}", UpdateCashFlow)
                .WithName("UpdateCashFlow")
                .WithOpenApi();

            group.MapDelete("/{id:guid}", DeleteCashFlow)
                .WithName("DeleteCashFlow")
                .WithOpenApi();

            // Query operations
            group.MapGet("/by-type", GetCashFlowsByType)
                .WithName("GetCashFlowsByType")
                .WithOpenApi();

            group.MapGet("/by-category", GetCashFlowsByCategory)
                .WithName("GetCashFlowsByCategory")
                .WithOpenApi();

            // Analysis operations
            group.MapGet("/summary", GetCashFlowSummary)
                .WithName("GetCashFlowSummary")
                .WithOpenApi();

            group.MapGet("/summary/monthly", GetMonthlyCashFlowSummary)
                .WithName("GetMonthlyCashFlowSummary")
                .WithOpenApi();

            group.MapGet("/summary/yearly", GetYearlyCashFlowSummary)
                .WithName("GetYearlyCashFlowSummary")
                .WithOpenApi();

            group.MapGet("/trend", GetCashFlowTrend)
                .WithName("GetCashFlowTrend")
                .WithOpenApi();

            // Projection operations
            group.MapGet("/projection", GetCashFlowProjection)
                .WithName("GetCashFlowProjection")
                .WithOpenApi();
        }

        #region Cash Flow Management

        private static async Task<IResult> CreateCashFlow(
            CreateCashFlowDto dto,
            ClaimsPrincipal user,
            ICashFlowService cashFlowService,
            ILogger<ICashFlowService> logger)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("Company ID not found in token");

                dto.CompanyId = companyId;
                var result = await cashFlowService.CreateCashFlowAsync(dto);
                return Results.Created($"/api/cashflow/{result.Id}", result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating cash flow");
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> GetCashFlowById(
            Guid id,
            ClaimsPrincipal user,
            ICashFlowService cashFlowService,
            ILogger<ICashFlowService> logger)
        {
            try
            {
                var result = await cashFlowService.GetCashFlowByIdAsync(id);
                if (result == null)
                    return Results.NoContent();

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting cash flow by ID: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> GetCashFlowsByCompany(
            ClaimsPrincipal user,
            ICashFlowService cashFlowService,
            ILogger<ICashFlowService> logger,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("Company ID not found in token");

                var result = await cashFlowService.GetCashFlowsByCompanyAsync(companyId, startDate, endDate);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting cash flows for company");
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> GetCashFlowsByType(
            ClaimsPrincipal user,
            ICashFlowService cashFlowService,
            ILogger<ICashFlowService> logger,
            CashFlowType type,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("Company ID not found in token");

                var result = await cashFlowService.GetCashFlowsByTypeAsync(companyId, type, startDate, endDate);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting cash flows by type");
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> GetCashFlowsByCategory(
            ClaimsPrincipal user,
            ICashFlowService cashFlowService,
            ILogger<ICashFlowService> logger,
            string category,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("Company ID not found in token");

                var result = await cashFlowService.GetCashFlowsByCategoryAsync(companyId, category, startDate, endDate);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting cash flows by category");
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> UpdateCashFlow(
            Guid id,
            CreateCashFlowDto dto,
            ClaimsPrincipal user,
            ICashFlowService cashFlowService,
            ILogger<ICashFlowService> logger)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("Company ID not found in token");

                dto.CompanyId = companyId;
                var result = await cashFlowService.UpdateCashFlowAsync(id, dto);
                if (result == null)
                    return Results.NoContent();

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating cash flow: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> DeleteCashFlow(
            Guid id,
            ClaimsPrincipal user,
            ICashFlowService cashFlowService,
            ILogger<ICashFlowService> logger)
        {
            try
            {
                var result = await cashFlowService.DeleteCashFlowAsync(id);
                if (!result)
                    return Results.NoContent();

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting cash flow: {Id}", id);
                return Results.Problem("Internal server error");
            }
        }

        #endregion

        #region Cash Flow Analysis

        private static async Task<IResult> GetCashFlowSummary(
            ClaimsPrincipal user,
            ICashFlowService cashFlowService,
            ILogger<ICashFlowService> logger,
            DateTime startDate,
            DateTime endDate)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("Company ID not found in token");

                var result = await cashFlowService.GetCashFlowSummaryAsync(companyId, startDate, endDate);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting cash flow summary");
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> GetMonthlyCashFlowSummary(
            ClaimsPrincipal user,
            ICashFlowService cashFlowService,
            ILogger<ICashFlowService> logger,
            int year,
            int month)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("Company ID not found in token");

                var result = await cashFlowService.GetMonthlyCashFlowSummaryAsync(companyId, year, month);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting monthly cash flow summary");
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> GetYearlyCashFlowSummary(
            ClaimsPrincipal user,
            ICashFlowService cashFlowService,
            ILogger<ICashFlowService> logger,
            int year)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("Company ID not found in token");

                var result = await cashFlowService.GetYearlyCashFlowSummaryAsync(companyId, year);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting yearly cash flow summary");
                return Results.Problem("Internal server error");
            }
        }

        private static async Task<IResult> GetCashFlowTrend(
            ClaimsPrincipal user,
            ICashFlowService cashFlowService,
            ILogger<ICashFlowService> logger,
            DateTime startDate,
            DateTime endDate,
            string period = "monthly")
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("Company ID not found in token");

                var result = await cashFlowService.GetCashFlowTrendAsync(companyId, startDate, endDate, period);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting cash flow trend");
                return Results.Problem("Internal server error");
            }
        }

        #endregion

        #region Cash Flow Projections

        private static async Task<IResult> GetCashFlowProjection(
            ClaimsPrincipal user,
            ICashFlowService cashFlowService,
            ILogger<ICashFlowService> logger,
            DateTime projectionDate,
            int months = 12)
        {
            try
            {
                var companyId = GetCompanyId(user);
                if (companyId == Guid.Empty)
                    return Results.BadRequest("Company ID not found in token");

                var result = await cashFlowService.GetCashFlowProjectionAsync(companyId, projectionDate, months);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting cash flow projection");
                return Results.Problem("Internal server error");
            }
        }

        #endregion

        #region Helper Methods

        private static Guid GetCompanyId(ClaimsPrincipal user)
        {
            var companyIdClaim = user.FindFirst("CompanyId")?.Value;
            return Guid.TryParse(companyIdClaim, out var companyId) ? companyId : Guid.Empty;
        }

        private static Guid GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        #endregion
    }
}