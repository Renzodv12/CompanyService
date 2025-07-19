using CompanyService.Core.Models.Sale;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Sale
{
    public class GetSuppliersQuery : IRequest<List<SupplierDto>>
    {
        public Guid CompanyId { get; set; }
        public string SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
