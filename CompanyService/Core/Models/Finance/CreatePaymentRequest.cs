using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Models.Finance
{
    public class CreatePaymentRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto del pago debe ser mayor a 0")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}