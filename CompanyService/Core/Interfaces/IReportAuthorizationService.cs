using CompanyService.Core.Enums;

namespace CompanyService.Core.Interfaces
{
    public interface IReportAuthorizationService
    {
        /// <summary>
        /// Verifica si el usuario tiene un permiso específico para reportes
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="companyId">ID de la empresa</param>
        /// <param name="permission">Permiso a verificar</param>
        /// <returns>True si el usuario tiene el permiso</returns>
        Task<bool> HasPermissionAsync(Guid userId, Guid companyId, ReportPermission permission);

        /// <summary>
        /// Verifica si el usuario puede acceder a un reporte específico
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="companyId">ID de la empresa</param>
        /// <param name="reportId">ID del reporte</param>
        /// <param name="action">Acción que se quiere realizar</param>
        /// <returns>True si el usuario puede realizar la acción</returns>
        Task<bool> CanAccessReportAsync(Guid userId, Guid companyId, Guid reportId, ReportPermission action);

        /// <summary>
        /// Verifica si el usuario puede ver las ejecuciones de reportes
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="companyId">ID de la empresa</param>
        /// <param name="targetUserId">ID del usuario cuyas ejecuciones se quieren ver (null para todas)</param>
        /// <returns>True si el usuario puede ver las ejecuciones</returns>
        Task<bool> CanViewExecutionsAsync(Guid userId, Guid companyId, Guid? targetUserId = null);

        /// <summary>
        /// Obtiene los permisos de reportes para un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="companyId">ID de la empresa</param>
        /// <returns>Lista de permisos del usuario</returns>
        Task<IEnumerable<ReportPermission>> GetUserPermissionsAsync(Guid userId, Guid companyId);

        /// <summary>
        /// Valida que el usuario tenga permisos para crear un reporte con las configuraciones especificadas
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="companyId">ID de la empresa</param>
        /// <param name="entityNames">Entidades que el reporte va a consultar</param>
        /// <returns>True si el usuario puede crear el reporte</returns>
        Task<bool> CanCreateReportWithEntitiesAsync(Guid userId, Guid companyId, IEnumerable<string> entityNames);
    }
}