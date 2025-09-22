using MediatR;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Querys.Procurement
{
    public class GetQuotationByIdQuery : IRequest<QuotationResponse?>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
    }
}