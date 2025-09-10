using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    /// <summary>
    /// Entidad que representa un usuario del sistema
    /// </summary>
    [Table("Users")]
    public class User
    {
        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email del usuario
        /// </summary>
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el usuario está activo
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Fecha de creación del usuario
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Empresas asociadas al usuario
        /// </summary>
        public virtual ICollection<UserCompany> UserCompanies { get; set; } = new List<UserCompany>();

        /// <summary>
        /// Logs de auditoría del usuario
        /// </summary>
        public virtual ICollection<ReportAuditLog> AuditLogs { get; set; } = new List<ReportAuditLog>();
    }
}