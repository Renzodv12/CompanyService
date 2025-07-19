using CompanyService.Core.Enums;

namespace CompanyService.Core.Models.Sale
{
    public class SaleDto
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public SaleStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsElectronicInvoice { get; set; }
    }
}
