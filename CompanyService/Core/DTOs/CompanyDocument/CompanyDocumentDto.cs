using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.CompanyDocument
{
    /// <summary>
    /// DTO para representar un documento de empresa
    /// </summary>
    public class CompanyDocumentDto
    {
        /// <summary>
        /// ID único del documento
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID de la empresa propietaria
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Nombre del documento
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Nombre original del archivo
        /// </summary>
        public string OriginalFileName { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de documento
        /// </summary>
        public CompanyDocumentType Type { get; set; }

        /// <summary>
        /// Ruta del archivo en el sistema
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Tipo MIME del archivo
        /// </summary>
        public string? MimeType { get; set; }

        /// <summary>
        /// Tamaño del archivo en bytes
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Hash del archivo para verificar integridad
        /// </summary>
        public string? FileHash { get; set; }

        /// <summary>
        /// Metadatos adicionales del documento (JSON)
        /// </summary>
        public string? Metadata { get; set; }

        /// <summary>
        /// Versión del documento
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// ID del documento padre (para versionado)
        /// </summary>
        public Guid? ParentDocumentId { get; set; }

        /// <summary>
        /// Nombre del documento padre
        /// </summary>
        public string? ParentDocumentName { get; set; }

        /// <summary>
        /// Fecha de expiración del documento
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Indica si el documento fue eliminado (soft delete)
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Usuario que subió el documento
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Nombre del usuario que subió el documento
        /// </summary>
        public string? CreatedByName { get; set; }

        /// <summary>
        /// Usuario que actualizó el documento por última vez
        /// </summary>
        public Guid? UpdatedBy { get; set; }

        /// <summary>
        /// URL para descargar el documento
        /// </summary>
        public string? DownloadUrl { get; set; }

        /// <summary>
        /// Indica si el documento está expirado
        /// </summary>
        public bool IsExpired => ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow;

        /// <summary>
        /// Tamaño del archivo formateado para mostrar
        /// </summary>
        public string FormattedSize
        {
            get
            {
                if (Size < 1024) return $"{Size} B";
                if (Size < 1024 * 1024) return $"{Size / 1024:F1} KB";
                if (Size < 1024 * 1024 * 1024) return $"{Size / (1024 * 1024):F1} MB";
                return $"{Size / (1024 * 1024 * 1024):F1} GB";
            }
        }
    }
}