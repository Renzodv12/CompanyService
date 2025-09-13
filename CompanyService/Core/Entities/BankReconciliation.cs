using CompanyService.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    public class BankReconciliation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid BankAccountId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StatementBalance { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BookBalance { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ReconciledBalance { get; set; }

        [Required]
        public DateTime StatementDate { get; set; }

        [Required]
        public DateTime ReconciliationDate { get; set; }

        [Required]
        public ReconciliationStatus Status { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string UpdatedBy { get; set; } = string.Empty;

        // Navigation properties
        public virtual BankAccount BankAccount { get; set; } = null!;
        public virtual ICollection<BankReconciliationItem> ReconciliationItems { get; set; } = new List<BankReconciliationItem>();
    }
}