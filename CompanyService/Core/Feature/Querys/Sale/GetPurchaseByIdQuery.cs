using MediatR;
using CompanyService.Core.Models.Sale;

namespace CompanyService.Core.Feature.Querys.Sale
{
    /// <summary>
    /// Query para obtener una compra por ID
    /// </summary>
    public class GetPurchaseByIdQuery : IRequest<PurchaseDetailDto?>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
    }
}
