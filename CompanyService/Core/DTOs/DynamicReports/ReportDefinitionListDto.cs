namespace CompanyService.Core.DTOs.DynamicReports
{
    /// <summary>
    /// DTO para lista de definiciones de reportes
    /// </summary>
    public class ReportDefinitionListDto
    {
        /// <summary>
        /// ID de la definición del reporte
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del reporte
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del reporte
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Entidad principal del reporte
        /// </summary>
        public string EntityName { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el reporte es compartido
        /// </summary>
        public bool IsShared { get; set; }

        /// <summary>
        /// Indica si el reporte está activo
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Versión del reporte
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última modificación
        /// </summary>
        public DateTime LastModifiedAt { get; set; }

        /// <summary>
        /// Nombre del usuario creador
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del usuario creador (alternativo)
        /// </summary>
        public string CreatedByUserName { get; set; } = string.Empty;

        /// <summary>
        /// Número de campos en el reporte
        /// </summary>
        public int FieldsCount { get; set; }

        /// <summary>
        /// Número de filtros en el reporte
        /// </summary>
        public int FiltersCount { get; set; }
    }
}