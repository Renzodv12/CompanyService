namespace CompanyService.Core.Enums
{
    /// <summary>
    /// Estados posibles de un backup
    /// </summary>
    public enum BackupStatus
    {
        /// <summary>
        /// Backup pendiente de iniciar
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Backup en progreso
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Backup completado exitosamente
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Backup falló
        /// </summary>
        Failed = 3,

        /// <summary>
        /// Backup cancelado
        /// </summary>
        Cancelled = 4,

        /// <summary>
        /// Backup expirado
        /// </summary>
        Expired = 5,

        /// <summary>
        /// Backup corrupto
        /// </summary>
        Corrupted = 6
    }
}