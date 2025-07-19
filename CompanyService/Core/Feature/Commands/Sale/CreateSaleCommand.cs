using CompanyService.Core.Enums;
using CompanyService.Core.Models.Sale;
using MediatR;

namespace CompanyService.Core.Feature.Commands.Sale
{
    public class CreateSaleCommand : IRequest<Guid>
    {
        public Guid CustomerId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string Notes { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool GenerateElectronicInvoice { get; set; }
        public List<SaleDetailItem> Items { get; set; } = new();
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
