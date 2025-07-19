using MediatR;

namespace CompanyService.Core.Feature.Commands.Sale
{
    public class CreateSupplierCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string DocumentNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ContactPerson { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
