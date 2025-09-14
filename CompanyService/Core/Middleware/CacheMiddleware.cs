using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace CompanyService.Core.Middleware
{
    /// <summary>
    /// Middleware para manejo centralizado de caché y logging de performance
    /// </summary>
    public class CacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CacheMiddleware> _logger;

        public CacheMiddleware(RequestDelegate next, ILogger<CacheMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var originalBodyStream = context.Response.Body;

            try
            {
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                // Ejecutar el siguiente middleware
                await _next(context);

                stopwatch.Stop();

                // Log de performance para endpoints cacheados
                if (context.Request.Headers.ContainsKey("X-Cache-Hit"))
                {
                    _logger.LogInformation(
                        "Cache HIT for {Method} {Path} - Duration: {Duration}ms",
                        context.Request.Method,
                        context.Request.Path,
                        stopwatch.ElapsedMilliseconds);
                }
                else if (IsGetRequest(context) && IsSuccessfulResponse(context))
                {
                    _logger.LogInformation(
                        "Cache MISS for {Method} {Path} - Duration: {Duration}ms - Status: {StatusCode}",
                        context.Request.Method,
                        context.Request.Path,
                        stopwatch.ElapsedMilliseconds,
                        context.Response.StatusCode);
                }

                // Log de invalidación de caché
                if (context.Items.ContainsKey("CacheInvalidated"))
                {
                    var invalidatedKeys = context.Items["CacheInvalidated"] as List<string>;
                    if (invalidatedKeys?.Any() == true)
                    {
                        _logger.LogInformation(
                            "Cache invalidated for {Method} {Path} - Keys: {Keys}",
                            context.Request.Method,
                            context.Request.Path,
                            string.Join(", ", invalidatedKeys));
                    }
                }

                // Copiar la respuesta de vuelta al stream original
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "Error processing request {Method} {Path} - Duration: {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private static bool IsGetRequest(HttpContext context)
        {
            return context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSuccessfulResponse(HttpContext context)
        {
            return context.Response.StatusCode >= 200 && context.Response.StatusCode < 300;
        }
    }

    /// <summary>
    /// Extensión para registrar el middleware de caché
    /// </summary>
    public static class CacheMiddlewareExtensions
    {
        public static IApplicationBuilder UseCacheMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CacheMiddleware>();
        }
    }
}