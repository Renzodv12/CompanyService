using System.ComponentModel;

namespace CompanyService.Core.Enums
{
    /// <summary>
    /// Acciones auditables en el sistema de reportes dinámicos
    /// </summary>
    public enum ReportAuditAction
    {
        [Description("Reporte creado")]
        ReportCreated = 1,

        [Description("Reporte actualizado")]
        ReportUpdated = 2,

        [Description("Reporte eliminado")]
        ReportDeleted = 3,

        [Description("Reporte visualizado")]
        ReportViewed = 4,

        [Description("Reporte ejecutado")]
        ReportExecuted = 5,

        [Description("Reporte exportado")]
        ReportExported = 6,

        [Description("Reporte compartido")]
        ReportShared = 7,

        [Description("Permisos de reporte modificados")]
        ReportPermissionsChanged = 8,

        [Description("Ejecución de reporte cancelada")]
        ReportExecutionCancelled = 9,

        [Description("Error en ejecución de reporte")]
        ReportExecutionFailed = 10,

        [Description("Acceso denegado a reporte")]
        ReportAccessDenied = 11,

        [Description("Configuración de reporte importada")]
        ReportConfigurationImported = 12,

        [Description("Configuración de reporte exportada")]
        ReportConfigurationExported = 13,

        [Description("Reporte duplicado")]
        ReportDuplicated = 14,

        [Description("Filtros de reporte aplicados")]
        ReportFiltersApplied = 15
    }
}