namespace CompanyService.Core.Models.Product
{
    public class UpdateStockRequest
    {
        public int Quantity { get; set; }
        public CompanyService.Core.Enums.MovementType MovementType { get; set; }
        public string Reason { get; set; }
        public string Reference { get; set; }
    }
}
