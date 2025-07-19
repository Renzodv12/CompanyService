using CompanyService.Core.Models.Customer;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Customer
{
    public class GetCustomersQuery : IRequest<List<CustomerDto>>
    {
        public Guid CompanyId { get; set; }
        public string SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
