namespace CompanyService.Core.Models.Report
{
    public class RecentSaleDto
    {
        public string SaleNumber { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }

}
