namespace CompanyService.Core.Entities
{
    public class Budget
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public int? Month { get; set; } // null para presupuesto anual
        public decimal BudgetedAmount { get; set; }
        public decimal ActualAmount { get; set; } = 0;
        public decimal Variance => ActualAmount - BudgetedAmount;
        public decimal VariancePercentage => BudgetedAmount != 0 ? (Variance / BudgetedAmount) * 100 : 0;
        public Guid? AccountId { get; set; } // Cuenta contable relacionada
        public string Category { get; set; }
        public string Type { get; set; } = "Monthly"; // Annual, Quarterly, Monthly, Project
        public int Status { get; set; } = 1; // 1=Draft, 2=Active, 3=Completed, 4=Cancelled
        public bool IsActive { get; set; } = true;
        public string Notes { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navegación
        public Account Account { get; set; }
        public Company Company { get; set; }
        public ICollection<BudgetLine> BudgetLines { get; set; } = new List<BudgetLine>();
    }
}