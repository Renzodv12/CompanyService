namespace CompanyService.Core.Models.Sale
{
    public class CreateSupplierRequest
    {
        public string Name { get; set; }
        public string DocumentNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ContactPerson { get; set; }
    }
}
