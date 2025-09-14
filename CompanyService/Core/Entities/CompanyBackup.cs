using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    /// <summary>
    /// Entidad que representa un backup de los datos de la empresa
    /// </summary>
    [Table("CompanyBackups")]
    public class CompanyBackup
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public BackupType Type { get; set; }

        [Required]
        public BackupStatus Status { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        public long Size { get; set; } // Tamaño en bytes

        [MaxLength(64)]
        public string? FileHash { get; set; } // Para verificar integridad

        // Metadatos del backup (JSON)
        [Column(TypeName = "text")]
        public string? Metadata { get; set; }

        // Información de progreso
        public int ProgressPercentage { get; set; } = 0;

        [MaxLength(500)]
        public string? ErrorMessage { get; set; }

        // Fechas importantes
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public Guid CreatedBy { get; set; }

        // Navegación
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;

        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } = null!;
    }
}