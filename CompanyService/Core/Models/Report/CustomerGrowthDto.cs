namespace CompanyService.Core.Models.Report
{
    public class CustomerGrowthDto
    {
        public string Period { get; set; } = string.Empty;
        public int NewCustomers { get; set; }
        public int TotalCustomers { get; set; }
        public decimal GrowthPercentage { get; set; }
    }
}
