namespace CompanyService.Core.Entities
{
    public class SaleDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Subtotal { get; set; }

        // Navegación
        public Sale Sale { get; set; }
        public Product Product { get; set; }
    }
}
