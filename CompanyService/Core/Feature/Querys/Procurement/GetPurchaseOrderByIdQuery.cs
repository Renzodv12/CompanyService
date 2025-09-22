using MediatR;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Querys.Procurement
{
    public class GetPurchaseOrderByIdQuery : IRequest<PurchaseOrderResponse?>
    {
        public Guid CompanyId { get; set; }
        public Guid Id { get; set; }
    }
}