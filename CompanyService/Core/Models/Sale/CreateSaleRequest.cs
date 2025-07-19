namespace CompanyService.Core.Models.Sale
{
    public class CreateSaleRequest
    {
        public Guid CustomerId { get; set; }
        public CompanyService.Core.Enums.PaymentMethod PaymentMethod { get; set; }
        public string Notes { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool GenerateElectronicInvoice { get; set; }
        public List<SaleDetailItem> Items { get; set; } = new();
    }
}
