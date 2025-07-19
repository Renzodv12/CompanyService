using MediatR;

namespace CompanyService.Core.Feature.Commands.Customer
{
    public class UpdateCustomerCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public bool IsActive { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
