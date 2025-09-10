using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CompanyService.Core.Attributes
{
    /// <summary>
    /// Atributo para validar permisos específicos de reportes
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class ReportPermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly ReportPermission _requiredPermission;
        private readonly bool _requireOwnership;

        public ReportPermissionAttribute(ReportPermission requiredPermission, bool requireOwnership = false)
        {
            _requiredPermission = requiredPermission;
            _requireOwnership = requireOwnership;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authService = context.HttpContext.RequestServices.GetService<IReportAuthorizationService>();
            if (authService == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var companyIdClaim = user.FindFirst("CompanyId")?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId) || !Guid.TryParse(companyIdClaim, out var companyId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Si requiere ownership, verificar acceso al reporte específico
            if (_requireOwnership)
            {
                var reportIdValue = context.RouteData.Values["id"]?.ToString();
                if (Guid.TryParse(reportIdValue, out var reportId))
                {
                    var canAccess = await authService.CanAccessReportAsync(userId, companyId, reportId, _requiredPermission);
                    if (!canAccess)
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
                else
                {
                    context.Result = new BadRequestResult();
                    return;
                }
            }
            else
            {
                // Verificar permiso general
                var hasPermission = await authService.HasPermissionAsync(userId, companyId, _requiredPermission);
                if (!hasPermission)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }
}