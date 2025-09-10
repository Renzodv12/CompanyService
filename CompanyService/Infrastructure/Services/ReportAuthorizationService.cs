using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Infrastructure.Services
{
    public class ReportAuthorizationService : IReportAuthorizationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportAuthorizationService> _logger;

        // Mapeo de permisos de reportes a acciones del sistema existente
        private readonly Dictionary<ReportPermission, string> _permissionMapping = new()
        {
            { ReportPermission.ViewReports, "Reports.View" },
            { ReportPermission.CreateReports, "Reports.Create" },
            { ReportPermission.EditOwnReports, "Reports.Edit" },
            { ReportPermission.EditAllReports, "Reports.EditAll" },
            { ReportPermission.DeleteOwnReports, "Reports.Delete" },
            { ReportPermission.DeleteAllReports, "Reports.DeleteAll" },
            { ReportPermission.ExecuteReports, "Reports.Execute" },
            { ReportPermission.ExportReports, "Reports.Export" },
            { ReportPermission.ViewExecutionHistory, "Reports.ViewHistory" },
            { ReportPermission.ViewAllExecutions, "Reports.ViewAllExecutions" },
            { ReportPermission.ManageReportPermissions, "Reports.ManagePermissions" }
        };

        public ReportAuthorizationService(ApplicationDbContext context, ILogger<ReportAuthorizationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> HasPermissionAsync(Guid userId, Guid companyId, ReportPermission permission)
        {
            try
            {
                if (!_permissionMapping.TryGetValue(permission, out var permissionAction))
                {
                    _logger.LogWarning("Permission mapping not found for {Permission}", permission);
                    return false;
                }

                // Verificar si el usuario tiene el permiso a través de sus roles
                var hasPermission = await _context.UserCompanys
                    .Where(uc => uc.UserId == userId && uc.CompanyId == companyId)
                    .SelectMany(uc => uc.Role.RolePermissions)
                    .AnyAsync(rp => rp.Permission.Key == permissionAction.ToString());

                return hasPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission {Permission} for user {UserId}", permission, userId);
                return false;
            }
        }

        public async Task<bool> CanAccessReportAsync(Guid userId, Guid companyId, Guid reportId, ReportPermission action)
        {
            try
            {
                // Verificar permiso base
                if (!await HasPermissionAsync(userId, companyId, action))
                {
                    return false;
                }

                // Para acciones que requieren ser propietario, verificar ownership
                if (action == ReportPermission.EditOwnReports || action == ReportPermission.DeleteOwnReports)
                {
                    var isOwner = await _context.ReportDefinitions
                        .AnyAsync(rd => rd.Id == reportId && rd.CreatedByUserId == userId && rd.CompanyId == companyId);

                    if (!isOwner)
                    {
                        // Verificar si tiene permisos para editar/eliminar todos los reportes
                        var alternativePermission = action == ReportPermission.EditOwnReports 
                            ? ReportPermission.EditAllReports 
                            : ReportPermission.DeleteAllReports;
                        
                        return await HasPermissionAsync(userId, companyId, alternativePermission);
                    }
                }

                // Verificar que el reporte pertenece a la empresa
                var reportExists = await _context.ReportDefinitions
                    .AnyAsync(rd => rd.Id == reportId && rd.CompanyId == companyId);

                return reportExists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking report access for user {UserId}, report {ReportId}", userId, reportId);
                return false;
            }
        }

        public async Task<bool> CanViewExecutionsAsync(Guid userId, Guid companyId, Guid? targetUserId = null)
        {
            try
            {
                // Si no se especifica un usuario objetivo, verificar permiso para ver historial
                if (targetUserId == null)
                {
                    return await HasPermissionAsync(userId, companyId, ReportPermission.ViewExecutionHistory);
                }

                // Si es el mismo usuario, puede ver sus propias ejecuciones
                if (targetUserId == userId)
                {
                    return await HasPermissionAsync(userId, companyId, ReportPermission.ViewExecutionHistory);
                }

                // Para ver ejecuciones de otros usuarios, necesita permiso especial
                return await HasPermissionAsync(userId, companyId, ReportPermission.ViewAllExecutions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking execution view permission for user {UserId}", userId);
                return false;
            }
        }

        public async Task<IEnumerable<ReportPermission>> GetUserPermissionsAsync(Guid userId, Guid companyId)
        {
            try
            {
                var userPermissions = await _context.UserCompanys
                    .Where(uc => uc.UserId == userId && uc.CompanyId == companyId)
                    .SelectMany(uc => uc.Role.RolePermissions)
                    .Where(rp => rp.Permission.Key.Contains("Report"))
                    .Select(rp => rp.Permission.Key)
                    .ToListAsync();

                var reportPermissions = new List<ReportPermission>();
                
                foreach (var mapping in _permissionMapping)
                {
                    if (userPermissions.Contains(mapping.Value))
                    {
                        reportPermissions.Add(mapping.Key);
                    }
                }

                return reportPermissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user permissions for user {UserId}", userId);
                return Enumerable.Empty<ReportPermission>();
            }
        }

        public async Task<bool> CanCreateReportWithEntitiesAsync(Guid userId, Guid companyId, IEnumerable<string> entityNames)
        {
            try
            {
                // Verificar permiso básico de creación
                if (!await HasPermissionAsync(userId, companyId, ReportPermission.CreateReports))
                {
                    return false;
                }

                // Aquí se pueden agregar validaciones adicionales según las entidades
                // Por ejemplo, verificar si el usuario tiene acceso a ciertas entidades sensibles
                var restrictedEntities = new[] { "UserCompany", "Role", "Permission" };
                var hasRestrictedEntities = entityNames.Any(e => restrictedEntities.Contains(e, StringComparer.OrdinalIgnoreCase));

                if (hasRestrictedEntities)
                {
                    // Solo administradores pueden crear reportes con entidades restringidas
                    return await HasPermissionAsync(userId, companyId, ReportPermission.ManageReportPermissions);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking entity access for user {UserId}", userId);
                return false;
            }
        }
    }
}