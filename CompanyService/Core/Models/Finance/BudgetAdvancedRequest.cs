using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Models.Finance
{
    public class DuplicateBudgetRequest
    {
        [Required]
        [StringLength(100)]
        public string NewName { get; set; } = string.Empty;

        public int? NewYear { get; set; }
        public int? NewMonth { get; set; }
        public DateTime? NewStartDate { get; set; }
        public DateTime? NewEndDate { get; set; }
    }

    public class CompareBudgetsRequest
    {
        [Required]
        [MinLength(2, ErrorMessage = "Se requieren al menos 2 presupuestos para comparar")]
        [MaxLength(10, ErrorMessage = "No se pueden comparar más de 10 presupuestos a la vez")]
        public List<Guid> BudgetIds { get; set; } = new();
    }
}
