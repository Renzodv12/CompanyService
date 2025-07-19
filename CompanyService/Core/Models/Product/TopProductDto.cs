namespace CompanyService.Core.Models.Product
{
    public class TopProductDto
    {
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TransactionCount { get; set; }
    }
}
