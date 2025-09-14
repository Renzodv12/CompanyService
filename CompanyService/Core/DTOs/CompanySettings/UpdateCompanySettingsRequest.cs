using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.CompanySettings
{
    /// <summary>
    /// DTO para actualizar las configuraciones de la empresa
    /// </summary>
    public class UpdateCompanySettingsRequest
    {
        /// <summary>
        /// Zona horaria de la empresa
        /// </summary>
        [StringLength(50, ErrorMessage = "La zona horaria no puede exceder 50 caracteres")]
        public string? Timezone { get; set; }

        /// <summary>
        /// Moneda principal de la empresa
        /// </summary>
        [StringLength(3, ErrorMessage = "La moneda debe tener exactamente 3 caracteres")]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "La moneda debe ser un código ISO de 3 letras mayúsculas")]
        public string? Currency { get; set; }

        /// <summary>
        /// Idioma principal de la empresa
        /// </summary>
        [StringLength(5, ErrorMessage = "El idioma no puede exceder 5 caracteres")]
        public string? Language { get; set; }

        /// <summary>
        /// Formato de fecha preferido
        /// </summary>
        [StringLength(20, ErrorMessage = "El formato de fecha no puede exceder 20 caracteres")]
        public string? DateFormat { get; set; }

        /// <summary>
        /// Formato de hora preferido
        /// </summary>
        [StringLength(10, ErrorMessage = "El formato de hora no puede exceder 10 caracteres")]
        public string? TimeFormat { get; set; }

        /// <summary>
        /// Formato de número preferido
        /// </summary>
        [StringLength(20, ErrorMessage = "El formato de número no puede exceder 20 caracteres")]
        public string? NumberFormat { get; set; }

        /// <summary>
        /// Mes de inicio del año fiscal (1-12)
        /// </summary>
        [Range(1, 12, ErrorMessage = "El mes de inicio del año fiscal debe estar entre 1 y 12")]
        public int? FiscalYearStartMonth { get; set; }

        /// <summary>
        /// Días laborales de la semana (JSON array)
        /// </summary>
        public string? WorkingDays { get; set; }

        /// <summary>
        /// Hora de inicio de la jornada laboral
        /// </summary>
        public TimeSpan? WorkingHoursStart { get; set; }

        /// <summary>
        /// Hora de fin de la jornada laboral
        /// </summary>
        public TimeSpan? WorkingHoursEnd { get; set; }

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
    }
}