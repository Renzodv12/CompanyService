using CompanyService.Core.Enums;
using CompanyService.Core.Models.Customer;

namespace CompanyService.Core.Models.Sale
{
    public class SaleDetailDto
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; }
        public CustomerDto Customer { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public SaleStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string Notes { get; set; }
        public bool IsElectronicInvoice { get; set; }
        public string ElectronicInvoiceId { get; set; }
        public List<SaleItemDto> Items { get; set; } = new();
    }
}
