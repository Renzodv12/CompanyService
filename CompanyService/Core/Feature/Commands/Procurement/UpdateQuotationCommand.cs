using MediatR;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    public class UpdateQuotationCommand : IRequest<QuotationResponse>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public UpdateQuotationRequest Request { get; set; } = new();
    }
}