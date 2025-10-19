using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyService.Core.Entities
{
    /// <summary>
    /// Entidad para el Plan de Cuentas Contables
    /// </summary>
    [Table("ChartOfAccounts")]
    public class ChartOfAccounts
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty; // Asset, Liability, Equity, Revenue, Expense

        public Guid? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual ChartOfAccounts? Parent { get; set; }

        public virtual ICollection<ChartOfAccounts> Children { get; set; } = new List<ChartOfAccounts>();

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Relaciones con otras entidades
        public virtual ICollection<BudgetLine> BudgetLines { get; set; } = new List<BudgetLine>();
    }
}
