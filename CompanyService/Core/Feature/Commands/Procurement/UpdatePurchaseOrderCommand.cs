using MediatR;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    public class UpdatePurchaseOrderCommand : IRequest<PurchaseOrderResponse>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public UpdatePurchaseOrderRequest Request { get; set; } = new();
    }
}