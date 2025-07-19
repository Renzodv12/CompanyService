namespace CompanyService.Core.Models.Report
{
    public class SalesReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageTicket { get; set; }
        public List<DailySalesDto> SalesByDay { get; set; } = new();
        public List<TopCustomerDto> TopCustomers { get; set; } = new();
    }
}
