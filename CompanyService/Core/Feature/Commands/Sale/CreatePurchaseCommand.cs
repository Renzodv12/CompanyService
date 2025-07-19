using CompanyService.Core.Models.Sale;
using MediatR;

namespace CompanyService.Core.Feature.Commands.Sale
{
    public class CreatePurchaseCommand : IRequest<Guid>
    {
        public Guid SupplierId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string Notes { get; set; }
        public List<PurchaseDetailItem> Items { get; set; } = new();
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
