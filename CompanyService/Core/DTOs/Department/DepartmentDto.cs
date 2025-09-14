namespace CompanyService.Core.DTOs.Department
{
    /// <summary>
    /// DTO para representar un departamento
    /// </summary>
    public class DepartmentDto
    {
        /// <summary>
        /// ID único del departamento
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del departamento
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// ID de la empresa a la que pertenece
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// ID de la sucursal a la que pertenece
        /// </summary>
        public Guid? BranchId { get; set; }

        /// <summary>
        /// Nombre de la sucursal
        /// </summary>
        public string? BranchName { get; set; }

        /// <summary>
        /// Descripción del departamento
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// ID del gerente del departamento
        /// </summary>
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// Nombre del gerente del departamento
        /// </summary>
        public string? ManagerName { get; set; }

        /// <summary>
        /// ID del departamento padre
        /// </summary>
        public Guid? ParentDepartmentId { get; set; }

        /// <summary>
        /// Nombre del departamento padre
        /// </summary>
        public string? ParentDepartmentName { get; set; }

        /// <summary>
        /// Presupuesto asignado al departamento
        /// </summary>
        public decimal? Budget { get; set; }

        /// <summary>
        /// Código único del departamento
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Indica si el departamento está activo
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Indica si el departamento fue eliminado (soft delete)
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Usuario que creó el departamento
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Usuario que actualizó el departamento por última vez
        /// </summary>
        public Guid? UpdatedBy { get; set; }

        /// <summary>
        /// Lista de subdepartamentos
        /// </summary>
        public List<DepartmentDto>? SubDepartments { get; set; }

        /// <summary>
        /// Número de empleados en el departamento
        /// </summary>
        public int EmployeeCount { get; set; }
    }
}