namespace CompanyService.Core.DTOs
{
    /// <summary>
    /// DTO para posición
    /// </summary>
    public class PositionDto
    {
        /// <summary>
        /// ID de la posición
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre de la posición
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la posición
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// ID del departamento al que pertenece la posición
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Nombre del departamento
        /// </summary>
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Salario base de la posición
        /// </summary>
        public decimal? BaseSalary { get; set; }

        /// <summary>
        /// Nivel de la posición (Junior, Senior, etc.)
        /// </summary>
        public string? Level { get; set; }

        /// <summary>
        /// Indica si la posición está activa
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}