using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Models.CompanyManagement
{
    /// <summary>
    /// DTO para actualizar una posición existente
    /// </summary>
    public class UpdatePositionRequest
    {
        /// <summary>
        /// Nombre de la posición
        /// </summary>
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string? Name { get; set; }

        /// <summary>
        /// Descripción de la posición
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Description { get; set; }

        /// <summary>
        /// ID del departamento al que pertenece la posición
        /// </summary>
        public Guid? DepartmentId { get; set; }

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
        public bool? IsActive { get; set; }
    }
}