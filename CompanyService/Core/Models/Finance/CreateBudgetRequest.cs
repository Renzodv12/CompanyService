using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Models.Finance
{
    public class CreateBudgetRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El monto total debe ser mayor o igual a 0")]
        public decimal TotalAmount { get; set; }

        [StringLength(50)]
        public string? Type { get; set; } = "Monthly"; // Annual, Quarterly, Monthly, Project

        public List<CreateBudgetLineRequest> Lines { get; set; } = new();
    }

    public class CreateBudgetLineRequest
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0")]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}