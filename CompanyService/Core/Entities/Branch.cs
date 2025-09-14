using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    /// <summary>
    /// Entidad que representa una sucursal o ubicación de la empresa
    /// </summary>
    [Table("Branches")]
    public class Branch
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid CompanyId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        // Dirección
        [MaxLength(300)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        // Información de contacto
        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        // Manager de la sucursal
        public Guid? ManagerId { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navegación
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;

        [ForeignKey("ManagerId")]
        public virtual User? Manager { get; set; }

        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}