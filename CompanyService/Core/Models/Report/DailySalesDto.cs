namespace CompanyService.Core.Models.Report
{
    public class DailySalesDto
    {
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
    }
}
