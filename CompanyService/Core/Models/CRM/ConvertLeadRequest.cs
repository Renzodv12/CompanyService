using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Models.CRM
{
    /// <summary>
    /// DTO para convertir un lead en una oportunidad
    /// </summary>
    public class ConvertLeadRequest
    {
        /// <summary>
        /// Nombre de la oportunidad
        /// </summary>
        [Required(ErrorMessage = "El nombre de la oportunidad es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres")]
        public string OpportunityName { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la oportunidad
        /// </summary>
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres")]
        public string? OpportunityDescription { get; set; }

        /// <summary>
        /// Valor estimado de la oportunidad
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public decimal? EstimatedValue { get; set; }

        /// <summary>
        /// Probabilidad de cierre (0-100)
        /// </summary>
        [Range(0, 100, ErrorMessage = "La probabilidad debe estar entre 0 y 100")]
        public int? Probability { get; set; }

        /// <summary>
        /// Fecha esperada de cierre
        /// </summary>
        public DateTime? ExpectedCloseDate { get; set; }

        /// <summary>
        /// ID del usuario asignado a la oportunidad
        /// </summary>
        public Guid? AssignedToUserId { get; set; }

        /// <summary>
        /// Notas adicionales sobre la conversión
        /// </summary>
        [StringLength(2000, ErrorMessage = "Las notas no pueden exceder los 2000 caracteres")]
        public string? Notes { get; set; }

        /// <summary>
        /// Indica si se debe crear un cliente a partir del lead
        /// </summary>
        public bool CreateCustomer { get; set; } = true;
    }
}