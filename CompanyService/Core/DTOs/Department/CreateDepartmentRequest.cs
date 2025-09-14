using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.Department
{
    /// <summary>
    /// DTO para crear un nuevo departamento
    /// </summary>
    public class CreateDepartmentRequest
    {
        /// <summary>
        /// Nombre del departamento
        /// </summary>
        [Required(ErrorMessage = "El nombre del departamento es requerido")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// ID de la sucursal a la que pertenece el departamento
        /// </summary>
        public Guid? BranchId { get; set; }

        /// <summary>
        /// Descripción del departamento
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Description { get; set; }

        /// <summary>
        /// ID del gerente del departamento
        /// </summary>
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// ID del departamento padre (para jerarquías)
        /// </summary>
        public Guid? ParentDepartmentId { get; set; }

        /// <summary>
        /// Presupuesto asignado al departamento
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "El presupuesto debe ser mayor o igual a 0")]
        public decimal? Budget { get; set; }

        /// <summary>
        /// Código único del departamento
        /// </summary>
        [StringLength(20, ErrorMessage = "El código no puede exceder 20 caracteres")]
        public string? Code { get; set; }

        /// <summary>
        /// Indica si el departamento está activo
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}