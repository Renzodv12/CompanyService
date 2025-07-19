namespace CompanyService.Core.Models.Report
{
    public class TopCustomerDto
    {
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
    }
}
