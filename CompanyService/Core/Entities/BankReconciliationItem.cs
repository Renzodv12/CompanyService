using CompanyService.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    public class BankReconciliationItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid BankReconciliationId { get; set; }

        public Guid? BankTransactionId { get; set; }

        [Required]
        public ReconciliationItemType Type { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public bool IsReconciled { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual BankReconciliation BankReconciliation { get; set; } = null!;
        public virtual BankTransaction? BankTransaction { get; set; }
    }
}