using MediatR;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    public class CreatePurchaseOrderCommand : IRequest<PurchaseOrderResponse>
    {
        public Guid CompanyId { get; set; }
        public CreatePurchaseOrderRequest Request { get; set; } = new();
    }
}