namespace CompanyService.Core.DTOs
{
    /// <summary>
    /// DTO detallado para empleado con información adicional
    /// </summary>
    public class EmployeeDetailDto
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
        /// Información detallada de la posición
        /// </summary>
        public PositionInfo Position { get; set; } = new();

        /// <summary>
        /// ID del departamento del empleado
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Información detallada del departamento
        /// </summary>
        public DepartmentInfo Department { get; set; } = new();

        /// <summary>
        /// ID de la sucursal del empleado
        /// </summary>
        public Guid? BranchId { get; set; }

        /// <summary>
        /// Información de la sucursal
        /// </summary>
        public BranchInfo? Branch { get; set; }

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
        /// Información del supervisor/manager
        /// </summary>
        public ManagerInfo? Manager { get; set; }

        /// <summary>
        /// Lista de empleados subordinados
        /// </summary>
        public List<SubordinateInfo> Subordinates { get; set; } = new();

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

        /// <summary>
        /// Años de experiencia en la empresa
        /// </summary>
        public int YearsOfService => DateTime.Now.Year - HireDate.Year;
    }

    /// <summary>
    /// Información de posición para el detalle del empleado
    /// </summary>
    public class PositionInfo
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
        /// Nivel de la posición
        /// </summary>
        public string? Level { get; set; }

        /// <summary>
        /// Salario base de la posición
        /// </summary>
        public decimal? BaseSalary { get; set; }
    }

    /// <summary>
    /// Información de departamento para el detalle del empleado
    /// </summary>
    public class DepartmentInfo
    {
        /// <summary>
        /// ID del departamento
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del departamento
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del departamento
        /// </summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// Información de sucursal para el detalle del empleado
    /// </summary>
    public class BranchInfo
    {
        /// <summary>
        /// ID de la sucursal
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre de la sucursal
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ciudad de la sucursal
        /// </summary>
        public string? City { get; set; }
    }

    /// <summary>
    /// Información del manager para el detalle del empleado
    /// </summary>
    public class ManagerInfo
    {
        /// <summary>
        /// ID del manager
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre completo del manager
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Email del manager
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Posición del manager
        /// </summary>
        public string PositionName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Información de subordinado
    /// </summary>
    public class SubordinateInfo
    {
        /// <summary>
        /// ID del subordinado
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre completo del subordinado
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Email del subordinado
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Posición del subordinado
        /// </summary>
        public string PositionName { get; set; } = string.Empty;
    }
}