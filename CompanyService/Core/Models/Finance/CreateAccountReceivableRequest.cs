using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Models.Finance
{
    public class CreateAccountReceivableRequest
    {
        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public Guid CustomerId { get; set; }

        public Guid? SaleId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto total debe ser mayor a 0")]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}