using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.Finance
{
    public class CreateCashFlowDto
    {
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public CashFlowType Type { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Amount { get; set; }

        [Required]
        public bool IsInflow { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        public Guid? RelatedAccountId { get; set; }

        public Guid? RelatedBankAccountId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public Guid CompanyId { get; set; }
    }
}