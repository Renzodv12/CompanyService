using MediatR;

namespace CompanyService.Core.Models.Product
{
    public class GetTopProductsQuery : IRequest<List<TopProductDto>>
    {
        public Guid CompanyId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Top { get; set; } = 10;
    }
}
