namespace CompanyService.Core.Entities
{
    public class PurchaseDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PurchaseId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Subtotal { get; set; }

        // Navegación
        public Purchase Purchase { get; set; }
        public Product Product { get; set; }
    }
}
