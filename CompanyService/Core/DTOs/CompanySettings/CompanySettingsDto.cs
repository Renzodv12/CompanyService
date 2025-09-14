namespace CompanyService.Core.DTOs.CompanySettings
{
    /// <summary>
    /// DTO para las configuraciones de la empresa
    /// </summary>
    public class CompanySettingsDto
    {
        /// <summary>
        /// ID único de las configuraciones
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID de la empresa
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Zona horaria de la empresa
        /// </summary>
        public string Timezone { get; set; } = "UTC";

        /// <summary>
        /// Moneda principal de la empresa
        /// </summary>
        public string Currency { get; set; } = "USD";

        /// <summary>
        /// Idioma principal de la empresa
        /// </summary>
        public string Language { get; set; } = "es";

        /// <summary>
        /// Formato de fecha preferido
        /// </summary>
        public string DateFormat { get; set; } = "dd/MM/yyyy";

        /// <summary>
        /// Formato de hora preferido
        /// </summary>
        public string TimeFormat { get; set; } = "HH:mm";

        /// <summary>
        /// Formato de número preferido
        /// </summary>
        public string NumberFormat { get; set; } = "#,##0.00";

        /// <summary>
        /// Mes de inicio del año fiscal (1-12)
        /// </summary>
        public int FiscalYearStartMonth { get; set; } = 1;

        /// <summary>
        /// Días laborales de la semana (JSON array)
        /// </summary>
        public string WorkingDays { get; set; } = "[1,2,3,4,5]"; // Lunes a Viernes

        /// <summary>
        /// Hora de inicio de la jornada laboral
        /// </summary>
        public TimeSpan WorkingHoursStart { get; set; } = new TimeSpan(8, 0, 0); // 08:00

        /// <summary>
        /// Hora de fin de la jornada laboral
        /// </summary>
        public TimeSpan WorkingHoursEnd { get; set; } = new TimeSpan(17, 0, 0); // 17:00

        /// <summary>
        /// Configuraciones de notificaciones (JSON)
        /// </summary>
        public string? NotificationSettings { get; set; }

        /// <summary>
        /// Configuraciones de facturación (JSON)
        /// </summary>
        public string? InvoicingSettings { get; set; }

        /// <summary>
        /// Configuraciones de backup (JSON)
        /// </summary>
        public string? BackupSettings { get; set; }

        /// <summary>
        /// Configuraciones de seguridad (JSON)
        /// </summary>
        public string? SecuritySettings { get; set; }

        /// <summary>
        /// Configuraciones de integración (JSON)
        /// </summary>
        public string? IntegrationSettings { get; set; }

        /// <summary>
        /// Configuraciones personalizadas adicionales (JSON)
        /// </summary>
        public string? CustomSettings { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Usuario que creó las configuraciones
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Usuario que actualizó las configuraciones por última vez
        /// </summary>
        public Guid? UpdatedBy { get; set; }
    }
}