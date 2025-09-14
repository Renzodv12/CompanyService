using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    /// <summary>
    /// Entidad que registra todas las acciones de auditoría en el sistema
    /// </summary>
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        public Guid? UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty; // CREATE, UPDATE, DELETE, LOGIN, etc.

        [Required]
        [MaxLength(100)]
        public string EntityType { get; set; } = string.Empty; // Company, User, Branch, etc.

        public Guid? EntityId { get; set; }

        [MaxLength(255)]
        public string? EntityName { get; set; }

        // Datos antes del cambio (JSON)
        [Column(TypeName = "text")]
        public string? OldValues { get; set; }

        // Datos después del cambio (JSON)
        [Column(TypeName = "text")]
        public string? NewValues { get; set; }

        // Información adicional (JSON)
        [Column(TypeName = "text")]
        public string? AdditionalData { get; set; }

        [Required]
        [MaxLength(45)]
        public string IpAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        [Required]
        public AuditLogLevel Level { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        public string? Description { get; set; }

        // Navegación
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}