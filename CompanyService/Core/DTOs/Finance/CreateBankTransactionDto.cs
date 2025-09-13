using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.Finance
{
    public class CreateBankTransactionDto
    {
        [Required]
        public Guid BankAccountId { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionNumber { get; set; } = string.Empty;

        [Required]
        public BankTransactionType Type { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        [StringLength(200)]
        public string? Payee { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public Guid? RelatedAccountId { get; set; }

        [Required]
        public Guid CompanyId { get; set; }
    }
}