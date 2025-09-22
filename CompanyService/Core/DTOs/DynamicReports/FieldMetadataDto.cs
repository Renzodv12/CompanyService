namespace CompanyService.Core.DTOs.DynamicReports
{
    /// <summary>
    /// DTO para metadatos de campo
    /// </summary>
    public class FieldMetadataDto
    {
        /// <summary>
        /// Nombre del campo
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Nombre para mostrar del campo
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de datos del campo
        /// </summary>
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el campo es requerido
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Indica si el campo es una clave primaria
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Indica si el campo es una clave foránea
        /// </summary>
        public bool IsForeignKey { get; set; }

        /// <summary>
        /// Longitud máxima del campo (para strings)
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Descripción del campo
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Indica si el campo está disponible para reportes
        /// </summary>
        public bool IsAvailable { get; set; } = true;
    }
}