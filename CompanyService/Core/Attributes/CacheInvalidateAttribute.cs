using Microsoft.AspNetCore.Mvc.Filters;
using CompanyService.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace CompanyService.Core.Attributes
{
    /// <summary>
    /// Atributo para invalidar caché automáticamente después de operaciones de escritura
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheInvalidateAttribute : ActionFilterAttribute
    {
        private readonly string _dataType;
        private readonly bool _invalidateCompanyData;
        private readonly bool _invalidateUserData;
        private readonly string[] _customPatterns;

        public CacheInvalidateAttribute(string dataType, bool invalidateCompanyData = true, bool invalidateUserData = false, params string[] customPatterns)
        {
            _dataType = dataType;
            _invalidateCompanyData = invalidateCompanyData;
            _invalidateUserData = invalidateUserData;
            _customPatterns = customPatterns ?? new string[0];
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Ejecutar la acción primero
            var executedContext = await next();
            
            // Solo invalidar si la operación fue exitosa
            if (executedContext.Result is OkObjectResult || 
                executedContext.Result is CreatedResult || 
                executedContext.Result is CreatedAtActionResult ||
                executedContext.Result is NoContentResult)
            {
                var cacheInvalidationService = context.HttpContext.RequestServices.GetRequiredService<ICacheInvalidationService>();
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CacheInvalidateAttribute>>();

                try
                {
                    // Invalidar datos de la empresa
                    if (_invalidateCompanyData && context.ActionArguments.ContainsKey("companyId"))
                    {
                        var companyId = (Guid)context.ActionArguments["companyId"];
                        await cacheInvalidationService.InvalidateCompanyDataAsync(companyId, _dataType);
                    }

                    // Invalidar datos del usuario
                    if (_invalidateUserData)
                    {
                        var userId = GetCurrentUserId(context.HttpContext);
                        if (userId != Guid.Empty)
                        {
                            await cacheInvalidationService.InvalidateUserDataAsync(userId, _dataType);
                        }
                    }

                    // Invalidar patrones personalizados
                    foreach (var pattern in _customPatterns)
                    {
                        var resolvedPattern = ResolvePattern(pattern, context);
                        await cacheInvalidationService.InvalidatePatternAsync(resolvedPattern);
                    }

                    logger.LogInformation("Cache invalidated for data type: {DataType}", _dataType);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error invalidating cache for data type: {DataType}", _dataType);
                }
            }
        }

        private Guid GetCurrentUserId(HttpContext httpContext)
        {
            var userIdClaim = httpContext.User?.FindFirst("sub")?.Value ?? 
                            httpContext.User?.FindFirst("userId")?.Value;
            
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private string ResolvePattern(string pattern, ActionExecutingContext context)
        {
            var resolvedPattern = pattern;

            // Reemplazar placeholders con valores reales
            if (pattern.Contains("{companyId}") && context.ActionArguments.ContainsKey("companyId"))
            {
                resolvedPattern = resolvedPattern.Replace("{companyId}", context.ActionArguments["companyId"].ToString());
            }

            if (pattern.Contains("{userId}"))
            {
                var userId = GetCurrentUserId(context.HttpContext);
                resolvedPattern = resolvedPattern.Replace("{userId}", userId.ToString());
            }

            if (pattern.Contains("{dataType}"))
            {
                resolvedPattern = resolvedPattern.Replace("{dataType}", _dataType);
            }

            return resolvedPattern;
        }
    }
}