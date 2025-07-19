using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class Purchase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PurchaseNumber { get; set; }
        public Guid SupplierId { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public DateTime DeliveryDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public PurchaseStatus Status { get; set; } = PurchaseStatus.Pending;
        public string InvoiceNumber { get; set; } // Número de factura del proveedor
        public string Notes { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Supplier Supplier { get; set; }
        public Company Company { get; set; }
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
    }
}
