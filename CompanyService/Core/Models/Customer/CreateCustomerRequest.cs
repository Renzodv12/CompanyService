namespace CompanyService.Core.Models.Customer
{
    public class CreateCustomerRequest
    {
        public string Name { get; set; }
        public string DocumentNumber { get; set; }
        public CompanyService.Core.Enums.DocumentType DocumentType { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
    }
}
