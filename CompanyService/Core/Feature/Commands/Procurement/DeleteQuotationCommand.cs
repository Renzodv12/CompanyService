using MediatR;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    public class DeleteQuotationCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
    }
}