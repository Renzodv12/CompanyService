using MediatR;

namespace CompanyService.Core.Feature.Commands.Sale
{
    public class CancelSaleCommand : IRequest<bool>
    {
        public Guid SaleId { get; set; }
        public string Reason { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
