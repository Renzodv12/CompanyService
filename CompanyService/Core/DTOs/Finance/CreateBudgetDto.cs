using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.Finance
{
    public class CreateBudgetDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(2020, 2050, ErrorMessage = "El año debe estar entre 2020 y 2050")]
        public int Year { get; set; }

        [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12")]
        public int? Month { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El monto presupuestado no puede ser negativo")]
        public decimal BudgetedAmount { get; set; }

        public Guid? AccountId { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public List<CreateBudgetLineDto> BudgetLines { get; set; } = new();
    }

    public class CreateBudgetLineDto
    {
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El monto presupuestado no puede ser negativo")]
        public decimal BudgetedAmount { get; set; }

        public Guid? AccountId { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}