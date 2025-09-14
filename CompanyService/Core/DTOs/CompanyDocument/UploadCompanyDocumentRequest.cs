using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.CompanyDocument
{
    /// <summary>
    /// DTO para subir un documento de empresa
    /// </summary>
    public class UploadCompanyDocumentRequest
    {
        /// <summary>
        /// Nombre del documento
        /// </summary>
        [Required(ErrorMessage = "El nombre del documento es requerido")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de documento
        /// </summary>
        [Required(ErrorMessage = "El tipo de documento es requerido")]
        public CompanyDocumentType Type { get; set; }

        /// <summary>
        /// Archivo a subir
        /// </summary>
        [Required(ErrorMessage = "El archivo es requerido")]
        public IFormFile File { get; set; } = null!;

        /// <summary>
        /// ID del documento padre (para versionado)
        /// </summary>
        public Guid? ParentDocumentId { get; set; }

        /// <summary>
        /// Fecha de expiración del documento
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Metadatos adicionales del documento (JSON)
        /// </summary>
        public string? Metadata { get; set; }

        /// <summary>
        /// Descripción del documento
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Description { get; set; }

        /// <summary>
        /// Tags o etiquetas del documento (separadas por comas)
        /// </summary>
        [StringLength(200, ErrorMessage = "Los tags no pueden exceder 200 caracteres")]
        public string? Tags { get; set; }
    }
}