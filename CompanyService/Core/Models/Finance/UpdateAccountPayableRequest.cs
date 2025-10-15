using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Models.Finance
{
    /// <summary>
    /// Request para actualizar una cuenta por pagar
    /// </summary>
    public class UpdateAccountPayableRequest
    {
        [Required]
        public Guid SupplierId { get; set; }

        [Required]
        [StringLength(100)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}

