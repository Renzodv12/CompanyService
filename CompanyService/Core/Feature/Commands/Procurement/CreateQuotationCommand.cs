using MediatR;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    public class CreateQuotationCommand : IRequest<QuotationResponse>
    {
        public Guid CompanyId { get; set; }
        public CreateQuotationRequest Request { get; set; } = new();
    }
}