namespace CompanyService.Core.Enums
{
    /// <summary>
    /// Permisos específicos para el módulo de reportes dinámicos
    /// </summary>
    public enum ReportPermission
    {
        /// <summary>
        /// Permiso para ver reportes
        /// </summary>
        ViewReports = 1,

        /// <summary>
        /// Permiso para crear nuevos reportes
        /// </summary>
        CreateReports = 2,

        /// <summary>
        /// Permiso para editar reportes propios
        /// </summary>
        EditOwnReports = 3,

        /// <summary>
        /// Permiso para editar cualquier reporte de la empresa
        /// </summary>
        EditAllReports = 4,

        /// <summary>
        /// Permiso para eliminar reportes propios
        /// </summary>
        DeleteOwnReports = 5,

        /// <summary>
        /// Permiso para eliminar cualquier reporte de la empresa
        /// </summary>
        DeleteAllReports = 6,

        /// <summary>
        /// Permiso para ejecutar reportes
        /// </summary>
        ExecuteReports = 7,

        /// <summary>
        /// Permiso para exportar reportes
        /// </summary>
        ExportReports = 8,

        /// <summary>
        /// Permiso para ver el historial de ejecuciones
        /// </summary>
        ViewExecutionHistory = 9,

        /// <summary>
        /// Permiso para ver ejecuciones de otros usuarios
        /// </summary>
        ViewAllExecutions = 10,

        /// <summary>
        /// Permiso para administrar permisos de reportes
        /// </summary>
        ManageReportPermissions = 11
    }
}