namespace CompanyService.Core.Enums
{
    /// <summary>
    /// Niveles de importancia para los logs de auditoría
    /// </summary>
    public enum AuditLogLevel
    {
        /// <summary>
        /// Información general
        /// </summary>
        Info = 0,

        /// <summary>
        /// Advertencia
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Error
        /// </summary>
        Error = 2,

        /// <summary>
        /// Crítico - requiere atención inmediata
        /// </summary>
        Critical = 3,

        /// <summary>
        /// Actividad de seguridad
        /// </summary>
        Security = 4,

        /// <summary>
        /// Actividad administrativa
        /// </summary>
        Administrative = 5
    }
}