namespace CompanyService.Core.Enums
{
    /// <summary>
    /// Tipos de backup disponibles
    /// </summary>
    public enum BackupType
    {
        /// <summary>
        /// Backup completo de todos los datos
        /// </summary>
        Full = 0,

        /// <summary>
        /// Backup incremental (solo cambios desde el último backup)
        /// </summary>
        Incremental = 1,

        /// <summary>
        /// Backup diferencial (cambios desde el último backup completo)
        /// </summary>
        Differential = 2,

        /// <summary>
        /// Backup manual iniciado por usuario
        /// </summary>
        Manual = 3,

        /// <summary>
        /// Backup automático programado
        /// </summary>
        Automatic = 4
    }
}