using CompanyService.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    /// <summary>
    /// Entidad para registrar auditoría de acciones en reportes dinámicos
    /// </summary>
    [Table("ReportAuditLogs")]
    public class ReportAuditLog
    {
        /// <summary>
        /// Identificador único del log de auditoría
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// ID del usuario que realizó la acción
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// ID de la empresa
        /// </summary>
        [Required]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Acción realizada
        /// </summary>
        [Required]
        public ReportAuditAction Action { get; set; }

        /// <summary>
        /// ID del reporte (opcional)
        /// </summary>
        public Guid? ReportId { get; set; }

        /// <summary>
        /// ID de la ejecución del reporte (opcional)
        /// </summary>
        public Guid? ExecutionId { get; set; }

        /// <summary>
        /// Detalles adicionales de la acción
        /// </summary>
        [MaxLength(1000)]
        public string? Details { get; set; }

        /// <summary>
        /// Dirección IP del usuario
        /// </summary>
        [MaxLength(45)] // IPv6 max length
        public string? IpAddress { get; set; }

        /// <summary>
        /// User agent del navegador
        /// </summary>
        [MaxLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// Timestamp de cuando ocurrió la acción
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }

        // Navigation properties
        /// <summary>
        /// Referencia al usuario que realizó la acción
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        /// <summary>
        /// Referencia a la empresa
        /// </summary>
        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Referencia al reporte (opcional)
        /// </summary>
        [ForeignKey(nameof(ReportId))]
        public virtual ReportDefinition? Report { get; set; }

        /// <summary>
        /// Referencia a la ejecución del reporte (opcional)
        /// </summary>
        [ForeignKey(nameof(ExecutionId))]
        public virtual ReportExecution? Execution { get; set; }
    }
}