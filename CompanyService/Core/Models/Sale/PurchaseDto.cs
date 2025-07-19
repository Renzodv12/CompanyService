using CompanyService.Core.Enums;

namespace CompanyService.Core.Models.Sale
{
    public class PurchaseDto
    {
        public Guid Id { get; set; }
        public string PurchaseNumber { get; set; }
        public string SupplierName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public PurchaseStatus Status { get; set; }
        public string InvoiceNumber { get; set; }
    }
}
