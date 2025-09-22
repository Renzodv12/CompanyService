namespace CompanyService.Core.DTOs.DynamicReports
{
    /// <summary>
    /// DTO para metadatos de entidad
    /// </summary>
    public class EntityMetadataDto
    {
        /// <summary>
        /// Nombre de la entidad
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Nombre para mostrar de la entidad
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la entidad
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Nombre de la tabla en la base de datos
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la entidad está disponible para reportes
        /// </summary>
        public bool IsAvailable { get; set; } = true;
    }
}