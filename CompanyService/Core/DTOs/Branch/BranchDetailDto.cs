namespace CompanyService.Core.DTOs.Branch
{
    /// <summary>
    /// DTO detallado para representar una sucursal con información adicional
    /// </summary>
    public class BranchDetailDto
    {
        /// <summary>
        /// ID único de la sucursal
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre de la sucursal
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// ID de la empresa a la que pertenece
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Nombre de la empresa
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Dirección completa de la sucursal
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Ciudad donde se ubica la sucursal
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Estado o provincia
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Código postal
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// País
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Teléfono de la sucursal
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Email de la sucursal
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// ID del gerente de la sucursal
        /// </summary>
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// Nombre del gerente de la sucursal
        /// </summary>
        public string? ManagerName { get; set; }

        /// <summary>
        /// Indica si la sucursal está activa
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Usuario que creó la sucursal
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Nombre del usuario que creó la sucursal
        /// </summary>
        public string CreatedByName { get; set; } = string.Empty;

        /// <summary>
        /// Usuario que actualizó la sucursal por última vez
        /// </summary>
        public Guid? UpdatedBy { get; set; }

        /// <summary>
        /// Nombre del usuario que actualizó la sucursal por última vez
        /// </summary>
        public string? UpdatedByName { get; set; }

        /// <summary>
        /// Número total de empleados en la sucursal
        /// </summary>
        public int TotalEmployees { get; set; }

        /// <summary>
        /// Número de departamentos en la sucursal
        /// </summary>
        public int TotalDepartments { get; set; }
    }
}