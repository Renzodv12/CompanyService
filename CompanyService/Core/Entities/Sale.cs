using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class Sale
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string SaleNumber { get; set; } // Número de venta secuencial
        public Guid CustomerId { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public SaleStatus Status { get; set; } = SaleStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }
        public string Notes { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; } // Usuario que realizó la venta
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Facturación electrónica
        public string ElectronicInvoiceId { get; set; } // ID del DTE
        public bool IsElectronicInvoice { get; set; } = false;

        // Navegación
        public Customer Customer { get; set; }
        public Company Company { get; set; }
        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}
