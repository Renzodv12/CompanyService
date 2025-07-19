namespace CompanyService.Core.Models.Report
{
    public class LowStockProductDto
    {
        public string Name { get; set; }
        public string SKU { get; set; }
        public int CurrentStock { get; set; }
        public int MinStock { get; set; }
    }
}
