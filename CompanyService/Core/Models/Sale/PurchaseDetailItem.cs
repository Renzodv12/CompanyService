namespace CompanyService.Core.Models.Sale
{
    public class PurchaseDetailItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}
