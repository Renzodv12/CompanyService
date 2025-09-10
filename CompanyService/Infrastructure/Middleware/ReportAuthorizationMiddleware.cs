using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace CompanyService.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware para manejo centralizado de autorización de reportes
    /// </summary>
    public class ReportAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ReportAuthorizationMiddleware> _logger;

        public ReportAuthorizationMiddleware(RequestDelegate next, ILogger<ReportAuthorizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Acceso no autorizado a recurso de reportes. Usuario: {UserId}, Ruta: {Path}",
                    context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anónimo",
                    context.Request.Path);

                await HandleUnauthorizedAsync(context, ex.Message);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("permission"))
            {
                _logger.LogWarning(ex, "Error de permisos en reportes. Usuario: {UserId}, Ruta: {Path}",
                    context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anónimo",
                    context.Request.Path);

                await HandleForbiddenAsync(context, ex.Message);
            }
        }

        private static async Task HandleUnauthorizedAsync(HttpContext context, string message)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "Unauthorized",
                message = "No tiene autorización para acceder a este recurso de reportes",
                details = message,
                timestamp = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }

        private static async Task HandleForbiddenAsync(HttpContext context, string message)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "Forbidden",
                message = "No tiene los permisos necesarios para realizar esta acción en reportes",
                details = message,
                timestamp = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    /// <summary>
    /// Extensión para registrar el middleware de autorización de reportes
    /// </summary>
    public static class ReportAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseReportAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ReportAuthorizationMiddleware>();
        }
    }
}