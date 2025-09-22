using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Models.CompanyManagement
{
    /// <summary>
    /// DTO para crear un nuevo empleado
    /// </summary>
    public class CreateEmployeeRequest
    {
        /// <summary>
        /// Nombre del empleado
        /// </summary>
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del empleado
        /// </summary>
        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Email del empleado
        /// </summary>
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Teléfono del empleado
        /// </summary>
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string? Phone { get; set; }

        /// <summary>
        /// ID de la posición del empleado
        /// </summary>
        [Required(ErrorMessage = "La posición es requerida")]
        public Guid PositionId { get; set; }

        /// <summary>
        /// ID del departamento del empleado
        /// </summary>
        [Required(ErrorMessage = "El departamento es requerido")]
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// ID de la sucursal del empleado
        /// </summary>
        [Required(ErrorMessage = "La sucursal es requerida")]
        public Guid BranchId { get; set; }

        /// <summary>
        /// Fecha de contratación
        /// </summary>
        [Required(ErrorMessage = "La fecha de contratación es requerida")]
        public DateTime HireDate { get; set; }

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
        public bool IsActive { get; set; } = true;
    }
}