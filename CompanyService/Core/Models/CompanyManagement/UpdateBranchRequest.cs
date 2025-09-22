using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Models.CompanyManagement
{
    /// <summary>
    /// DTO para actualizar una sucursal existente
    /// </summary>
    public class UpdateBranchRequest
    {
        /// <summary>
        /// Nombre de la sucursal
        /// </summary>
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string? Name { get; set; }

        /// <summary>
        /// Descripción de la sucursal
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Description { get; set; }

        /// <summary>
        /// Dirección de la sucursal
        /// </summary>
        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
        public string? Address { get; set; }

        /// <summary>
        /// Ciudad de la sucursal
        /// </summary>
        [StringLength(100, ErrorMessage = "La ciudad no puede exceder los 100 caracteres")]
        public string? City { get; set; }

        /// <summary>
        /// Estado o provincia de la sucursal
        /// </summary>
        [StringLength(100, ErrorMessage = "El estado no puede exceder los 100 caracteres")]
        public string? State { get; set; }

        /// <summary>
        /// Código postal de la sucursal
        /// </summary>
        [StringLength(20, ErrorMessage = "El código postal no puede exceder los 20 caracteres")]
        public string? PostalCode { get; set; }

        /// <summary>
        /// País de la sucursal
        /// </summary>
        [StringLength(100, ErrorMessage = "El país no puede exceder los 100 caracteres")]
        public string? Country { get; set; }

        /// <summary>
        /// Teléfono de la sucursal
        /// </summary>
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string? Phone { get; set; }

        /// <summary>
        /// Email de la sucursal
        /// </summary>
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string? Email { get; set; }

        /// <summary>
        /// ID del manager de la sucursal
        /// </summary>
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// Indica si la sucursal está activa
        /// </summary>
        public bool? IsActive { get; set; }
    }
}