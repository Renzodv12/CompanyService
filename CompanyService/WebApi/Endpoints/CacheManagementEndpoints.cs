using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Models.Cache;
using CompanyService.WebApi.Extensions;

namespace CompanyService.WebApi.Endpoints
{
    public static class CacheManagementEndpoints
    {
        public static IEndpointRouteBuilder MapCacheManagementEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/companies/{companyId:guid}/cache")
                .WithTags("Cache Management")
                .RequireAuthorization();

            // Obtener información del cache
            group.MapGet("/info/{key}", GetCacheInfo)
                .WithName("GetCacheInfo")
                .WithSummary("Get cache information")
                .WithDescription("Retrieves information about a specific cache key")
                .Produces<CacheInfo>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithOpenApi();

            // Obtener información del cache por patrón
            group.MapGet("/info", GetCacheInfoByPattern)
                .WithName("GetCacheInfoByPattern")
                .WithSummary("Get cache information by pattern")
                .WithDescription("Retrieves information about cache keys matching a pattern")
                .Produces<List<CacheInfo>>(StatusCodes.Status200OK)
                .WithOpenApi();

            // Invalidar cache específico
            group.MapDelete("/{key}", InvalidateCache)
                .WithName("InvalidateCache")
                .WithSummary("Invalidate specific cache")
                .WithDescription("Removes a specific cache key")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithOpenApi();

            // Invalidar cache por patrón
            group.MapDelete("/pattern/{pattern}", InvalidateCacheByPattern)
                .WithName("InvalidateCacheByPattern")
                .WithSummary("Invalidate cache by pattern")
                .WithDescription("Removes all cache keys matching a pattern")
                .Produces<object>(StatusCodes.Status200OK)
                .WithOpenApi();

            // Invalidar todo el cache de la empresa
            group.MapDelete("/company", InvalidateCompanyCache)
                .WithName("InvalidateCompanyCache")
                .WithSummary("Invalidate all company cache")
                .WithDescription("Removes all cache related to the company")
                .Produces<object>(StatusCodes.Status200OK)
                .WithOpenApi();

            // Invalidar cache por tipo de dato
            group.MapDelete("/data-type/{dataType}", InvalidateCacheByDataType)
                .WithName("InvalidateCacheByDataType")
                .WithSummary("Invalidate cache by data type")
                .WithDescription("Removes all cache related to a specific data type")
                .Produces<object>(StatusCodes.Status200OK)
                .WithOpenApi();

            // Obtener estadísticas del cache
            group.MapGet("/stats", GetCacheStats)
                .WithName("GetCacheStats")
                .WithSummary("Get cache statistics")
                .WithDescription("Retrieves cache statistics and metrics")
                .Produces<object>(StatusCodes.Status200OK)
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> GetCacheInfo(
            Guid companyId,
            string key,
            HttpContext httpContext,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheInfo = await cacheService.GetCacheInfoAsync(key);
                if (!cacheInfo.Exists)
                    return Results.NotFound(new { message = "Cache key not found" });

                return Results.Ok(cacheInfo);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetCacheInfoByPattern(
            Guid companyId,
            HttpContext httpContext,
            ICacheService cacheService,
            string pattern = "*")
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheInfos = await cacheService.GetCacheInfoByPatternAsync(pattern);
                return Results.Ok(cacheInfos);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> InvalidateCache(
            Guid companyId,
            string key,
            HttpContext httpContext,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var result = await cacheService.RemoveAsync(key);
                if (!result)
                    return Results.NotFound(new { message = "Cache key not found" });

                return Results.Ok(new { message = "Cache invalidated successfully" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> InvalidateCacheByPattern(
            Guid companyId,
            string pattern,
            HttpContext httpContext,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var deletedCount = await cacheService.RemoveByPatternAsync(pattern);
                return Results.Ok(new { 
                    message = "Cache invalidated successfully", 
                    deletedKeys = deletedCount 
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> InvalidateCompanyCache(
            Guid companyId,
            HttpContext httpContext,
            ICacheInvalidationService cacheInvalidationService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                await cacheInvalidationService.InvalidateAllCompanyDataAsync(companyId);
                return Results.Ok(new { message = "All company cache invalidated successfully" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> InvalidateCacheByDataType(
            Guid companyId,
            string dataType,
            HttpContext httpContext,
            ICacheInvalidationService cacheInvalidationService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                await cacheInvalidationService.InvalidateCompanyDataAsync(companyId, dataType);
                return Results.Ok(new { 
                    message = $"Cache invalidated successfully for data type: {dataType}" 
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetCacheStats(
            Guid companyId,
            HttpContext httpContext,
            IRedisService redisService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var dbSize = await redisService.GetDatabaseSizeAsync();
                
                var stats = new
                {
                    TotalKeys = dbSize,
                    CompanyId = companyId,
                    Timestamp = DateTime.UtcNow,
                    Database = "Redis"
                };

                return Results.Ok(stats);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
}
