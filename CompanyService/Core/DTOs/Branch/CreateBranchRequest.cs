using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.Branch
{
    /// <summary>
    /// DTO para crear una nueva sucursal
    /// </summary>
    public class CreateBranchRequest
    {
        /// <summary>
        /// Nombre de la sucursal
        /// </summary>
        [Required(ErrorMessage = "El nombre de la sucursal es requerido")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de la sucursal
        /// </summary>
        [StringLength(500, ErrorMessage = "La dirección no puede exceder 500 caracteres")]
        public string? Address { get; set; }

        /// <summary>
        /// Ciudad donde se ubica la sucursal
        /// </summary>
        [StringLength(100, ErrorMessage = "La ciudad no puede exceder 100 caracteres")]
        public string? City { get; set; }

        /// <summary>
        /// Estado o provincia
        /// </summary>
        [StringLength(100, ErrorMessage = "El estado no puede exceder 100 caracteres")]
        public string? State { get; set; }

        /// <summary>
        /// Código postal
        /// </summary>
        [StringLength(20, ErrorMessage = "El código postal no puede exceder 20 caracteres")]
        public string? PostalCode { get; set; }

        /// <summary>
        /// País
        /// </summary>
        [StringLength(100, ErrorMessage = "El país no puede exceder 100 caracteres")]
        public string? Country { get; set; }

        /// <summary>
        /// Teléfono de la sucursal
        /// </summary>
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string? Phone { get; set; }

        /// <summary>
        /// Email de la sucursal
        /// </summary>
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(255, ErrorMessage = "El email no puede exceder 255 caracteres")]
        public string? Email { get; set; }

        /// <summary>
        /// ID del gerente de la sucursal
        /// </summary>
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// Indica si la sucursal está activa
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}