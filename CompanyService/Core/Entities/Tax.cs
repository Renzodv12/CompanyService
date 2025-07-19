namespace CompanyService.Core.Entities
{
    public class Tax
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } // IVA, Impuesto al Valor Agregado
        public decimal Rate { get; set; } // Porcentaje (10%, 5%)
        public bool IsActive { get; set; } = true;
        public Guid CompanyId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Company Company { get; set; }
    }
}
