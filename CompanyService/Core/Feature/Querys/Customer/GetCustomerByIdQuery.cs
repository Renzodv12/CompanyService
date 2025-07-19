using CompanyService.Core.Models.Customer;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Customer
{
    public class GetCustomerByIdQuery : IRequest<CustomerDto?>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
    }
}
