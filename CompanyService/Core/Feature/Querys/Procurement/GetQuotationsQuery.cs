using MediatR;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Querys.Procurement
{
    public class GetQuotationsQuery : IRequest<List<QuotationResponse>>
    {
        public Guid CompanyId { get; set; }
        public QuotationFilterRequest? Filter { get; set; }
    }
}