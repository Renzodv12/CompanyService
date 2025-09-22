using MediatR;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Querys.Procurement
{
    public class GetPurchaseOrdersQuery : IRequest<List<PurchaseOrderResponse>>
    {
        public Guid CompanyId { get; set; }
        public PurchaseOrderFilterRequest? Filter { get; set; }
    }
}