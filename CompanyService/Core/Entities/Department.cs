using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    /// <summary>
    /// Entidad que representa un departamento dentro de la empresa
    /// </summary>
    [Table("Departments")]
    public class Department
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid CompanyId { get; set; }

        public Guid? BranchId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        // Manager del departamento
        public Guid? ManagerId { get; set; }

        // Departamento padre (para jerarquías)
        public Guid? ParentDepartmentId { get; set; }

        // Presupuesto asignado
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Budget { get; set; }

        // Código del departamento
        [MaxLength(20)]
        public string? Code { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navegación
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;

        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; }

        [ForeignKey("ManagerId")]
        public virtual User? Manager { get; set; }

        [ForeignKey("ParentDepartmentId")]
        public virtual Department? ParentDepartment { get; set; }

        public virtual ICollection<Department> SubDepartments { get; set; } = new List<Department>();
        public virtual ICollection<User> Employees { get; set; } = new List<User>();
    }
}