using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    /// <summary>
    /// Entidad que representa un documento de la empresa
    /// </summary>
    [Table("CompanyDocuments")]
    public class CompanyDocument
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required]
        public CompanyDocumentType Type { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string MimeType { get; set; } = string.Empty;

        public long Size { get; set; } // Tamaño en bytes

        [MaxLength(64)]
        public string? FileHash { get; set; } // Para verificar integridad

        // Metadatos adicionales (JSON)
        [Column(TypeName = "text")]
        public string? Metadata { get; set; }

        // Versionado
        public int Version { get; set; } = 1;
        public Guid? ParentDocumentId { get; set; }

        // Fecha de expiración (para documentos temporales)
        public DateTime? ExpiresAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid UploadedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navegación
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;

        [ForeignKey("UploadedBy")]
        public virtual User UploadedByUser { get; set; } = null!;

        [ForeignKey("ParentDocumentId")]
        public virtual CompanyDocument? ParentDocument { get; set; }

        public virtual ICollection<CompanyDocument> ChildDocuments { get; set; } = new List<CompanyDocument>();
    }
}