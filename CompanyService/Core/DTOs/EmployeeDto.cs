namespace CompanyService.Core.DTOs
{
    /// <summary>
    /// DTO para empleado
    /// </summary>
    public class EmployeeDto
    {
        /// <summary>
        /// ID del empleado
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del empleado
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del empleado
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Nombre completo del empleado
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Email del empleado
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Teléfono del empleado
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// ID de la posición del empleado
        /// </summary>
        public Guid PositionId { get; set; }

        /// <summary>
        /// Nombre de la posición
        /// </summary>
        public string PositionName { get; set; } = string.Empty;

        /// <summary>
        /// ID del departamento del empleado
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Nombre del departamento
        /// </summary>
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de contratación
        /// </summary>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// Salario del empleado
        /// </summary>
        public decimal? Salary { get; set; }

        /// <summary>
        /// ID del supervisor/manager
        /// </summary>
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// Nombre del supervisor/manager
        /// </summary>
        public string? ManagerName { get; set; }

        /// <summary>
        /// Indica si el empleado está activo
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