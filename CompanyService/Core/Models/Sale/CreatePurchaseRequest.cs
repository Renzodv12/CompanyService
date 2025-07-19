namespace CompanyService.Core.Models.Sale
{
    public class CreatePurchaseRequest
    {
        public Guid SupplierId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string Notes { get; set; }
        public List<PurchaseDetailItem> Items { get; set; } = new();
    }
}
