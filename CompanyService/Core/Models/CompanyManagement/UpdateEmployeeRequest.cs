using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Models.CompanyManagement
{
    /// <summary>
    /// DTO para actualizar un empleado existente
    /// </summary>
    public class UpdateEmployeeRequest
    {
        /// <summary>
        /// Nombre del empleado
        /// </summary>
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Apellido del empleado
        /// </summary>
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres")]
        public string? LastName { get; set; }

        /// <summary>
        /// Email del empleado
        /// </summary>
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string? Email { get; set; }

        /// <summary>
        /// Teléfono del empleado
        /// </summary>
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string? Phone { get; set; }

        /// <summary>
        /// ID de la posición del empleado
        /// </summary>
        public Guid? PositionId { get; set; }

        /// <summary>
        /// ID del departamento del empleado
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// ID de la sucursal del empleado
        /// </summary>
        public Guid? BranchId { get; set; }

        /// <summary>
        /// Fecha de contratación
        /// </summary>
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// Salario del empleado
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser mayor o igual a 0")]
        public decimal? Salary { get; set; }

        /// <summary>
        /// ID del supervisor/manager
        /// </summary>
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// Indica si el empleado está activo
        /// </summary>
        public bool? IsActive { get; set; }
    }
}