namespace CompanyService.Core.Entities
{
    public class BudgetLine
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BudgetId { get; set; }
        public string LineItem { get; set; }
        public string Description { get; set; }
        public decimal BudgetedAmount { get; set; }
        public decimal ActualAmount { get; set; } = 0;
        public decimal Variance => ActualAmount - BudgetedAmount;
        public int SortOrder { get; set; }
        public Guid? AccountId { get; set; } // Cuenta contable específica
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navegación
        public Budget Budget { get; set; }
        public Account Account { get; set; }
    }
}