using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Models.CompanyManagement
{
    /// <summary>
    /// DTO para crear una nueva posición
    /// </summary>
    public class CreatePositionRequest
    {
        /// <summary>
        /// Nombre de la posición
        /// </summary>
        [Required(ErrorMessage = "El nombre de la posición es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la posición
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Description { get; set; }

        /// <summary>
        /// ID del departamento al que pertenece la posición
        /// </summary>
        [Required(ErrorMessage = "El departamento es requerido")]
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Salario base de la posición
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser mayor o igual a 0")]
        public decimal? BaseSalary { get; set; }

        /// <summary>
        /// Nivel de la posición (Junior, Senior, etc.)
        /// </summary>
        [StringLength(50, ErrorMessage = "El nivel no puede exceder los 50 caracteres")]
        public string? Level { get; set; }

        /// <summary>
        /// Indica si la posición está activa
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}