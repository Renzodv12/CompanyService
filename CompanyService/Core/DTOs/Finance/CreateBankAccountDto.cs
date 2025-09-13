using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.Finance
{
    public class CreateBankAccountDto
    {
        [Required]
        [StringLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string BankName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string AccountName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? AccountType { get; set; }

        [StringLength(20)]
        public string? SwiftCode { get; set; }

        [StringLength(50)]
        public string? RoutingNumber { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El saldo inicial no puede ser negativo")]
        public decimal InitialBalance { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public Guid CompanyId { get; set; }
    }
}