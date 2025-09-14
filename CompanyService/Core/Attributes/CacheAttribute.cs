using Microsoft.AspNetCore.Mvc.Filters;
using CompanyService.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;

namespace CompanyService.Core.Attributes
{
    /// <summary>
    /// Atributo para cachear automáticamente las respuestas de los endpoints
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheAttribute : ActionFilterAttribute
    {
        private readonly int _durationInMinutes;
        private readonly string _cacheKeyPrefix;
        private readonly bool _varyByCompany;
        private readonly bool _varyByUser;

        public CacheAttribute(int durationInMinutes = 30, string cacheKeyPrefix = "", bool varyByCompany = true, bool varyByUser = false)
        {
            _durationInMinutes = durationInMinutes;
            _cacheKeyPrefix = cacheKeyPrefix;
            _varyByCompany = varyByCompany;
            _varyByUser = varyByUser;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var redisService = context.HttpContext.RequestServices.GetRequiredService<IRedisService>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CacheAttribute>>();

            try
            {
                var cacheKey = GenerateCacheKey(context);
                
                // Intentar obtener del caché
                var cachedResult = await redisService.GetAsync<object>(cacheKey);
                if (cachedResult != null)
                {
                    logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                    context.Result = new OkObjectResult(cachedResult);
                    return;
                }

                // Ejecutar la acción
                var executedContext = await next();

                // Cachear el resultado si es exitoso
                if (executedContext.Result is OkObjectResult okResult && okResult.Value != null)
                {
                    await redisService.SetAsync(cacheKey, okResult.Value, TimeSpan.FromMinutes(_durationInMinutes));
                    logger.LogInformation("Cached result for key: {CacheKey} with duration: {Duration} minutes", cacheKey, _durationInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in cache attribute");
                // Si hay error en caché, continuar sin caché
                await next();
            }
        }

        private string GenerateCacheKey(ActionExecutingContext context)
        {
            var keyBuilder = new StringBuilder();
            
            // Prefijo personalizado o nombre del controlador/acción
            if (!string.IsNullOrEmpty(_cacheKeyPrefix))
            {
                keyBuilder.Append(_cacheKeyPrefix);
            }
            else
            {
                keyBuilder.Append($"{context.Controller.GetType().Name}:{context.ActionDescriptor.DisplayName}");
            }

            // Agregar companyId si está configurado
            if (_varyByCompany && context.ActionArguments.ContainsKey("companyId"))
            {
                keyBuilder.Append($":company:{context.ActionArguments["companyId"]}");
            }

            // Agregar userId si está configurado
            if (_varyByUser)
            {
                var userId = context.HttpContext.User?.FindFirst("sub")?.Value ?? 
                           context.HttpContext.User?.FindFirst("userId")?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    keyBuilder.Append($":user:{userId}");
                }
            }

            // Agregar parámetros de query relevantes
            var queryParams = context.HttpContext.Request.Query
                .Where(q => !string.IsNullOrEmpty(q.Value))
                .OrderBy(q => q.Key)
                .Select(q => $"{q.Key}={q.Value}");

            if (queryParams.Any())
            {
                keyBuilder.Append($":query:{string.Join("&", queryParams)}");
            }

            // Generar hash para claves muy largas
            var key = keyBuilder.ToString();
            if (key.Length > 200)
            {
                using var sha256 = SHA256.Create();
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                key = $"{_cacheKeyPrefix}:hash:{Convert.ToBase64String(hash)}";
            }

            return key;
        }
    }
}