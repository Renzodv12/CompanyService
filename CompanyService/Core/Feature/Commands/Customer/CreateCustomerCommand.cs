using CompanyService.Core.Enums;
using MediatR;

namespace CompanyService.Core.Feature.Commands.Customer
{
    public class CreateCustomerCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string DocumentNumber { get; set; }
        public DocumentType DocumentType { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
