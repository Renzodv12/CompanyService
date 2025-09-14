using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    /// <summary>
    /// Entidad que representa las configuraciones generales de una empresa
    /// </summary>
    [Table("CompanySettings")]
    public class CompanySettings
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        // Configuraciones regionales
        [MaxLength(50)]
        public string Timezone { get; set; } = "UTC";

        [MaxLength(10)]
        public string Currency { get; set; } = "USD";

        [MaxLength(10)]
        public string Language { get; set; } = "en";

        [MaxLength(20)]
        public string DateFormat { get; set; } = "yyyy-MM-dd";

        [MaxLength(20)]
        public string TimeFormat { get; set; } = "HH:mm";

        [MaxLength(20)]
        public string NumberFormat { get; set; } = "#,##0.00";

        // Configuraciones fiscales
        [MaxLength(10)]
        public string FiscalYearStart { get; set; } = "01-01";

        public int FiscalYearStartMonth { get; set; } = 1;

        // Días laborales (JSON array)
        [MaxLength(100)]
        public string WorkingDays { get; set; } = "[1,2,3,4,5]"; // Lunes a Viernes

        // Horarios laborales
        public TimeSpan WorkingHoursStart { get; set; } = TimeSpan.FromHours(9);

        public TimeSpan WorkingHoursEnd { get; set; } = TimeSpan.FromHours(17);

        // Configuraciones de notificaciones (JSON)
        [Column(TypeName = "text")]
        public string? NotificationSettings { get; set; }

        // Configuraciones de seguridad (JSON)
        [Column(TypeName = "text")]
        public string? SecuritySettings { get; set; }

        // Configuraciones de integración (JSON)
        [Column(TypeName = "text")]
        public string? IntegrationSettings { get; set; }

        // Configuraciones personalizadas (JSON)
        [Column(TypeName = "text")]
        public string? CustomSettings { get; set; }

        // Configuraciones de facturación
        [MaxLength(20)]
        public string InvoicePrefix { get; set; } = "INV";

        [MaxLength(20)]
        public string InvoiceNumbering { get; set; } = "sequential"; // sequential, custom

        public int PaymentTerms { get; set; } = 30; // días

        // Configuraciones de backup
        public bool AutoBackupEnabled { get; set; } = false;
        public int BackupRetentionDays { get; set; } = 30;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navegación
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
    }
}