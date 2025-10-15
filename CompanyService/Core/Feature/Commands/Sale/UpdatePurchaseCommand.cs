using MediatR;
using CompanyService.Core.Models.Sale;

namespace CompanyService.Core.Feature.Commands.Sale
{
    /// <summary>
    /// Comando para actualizar una compra existente
    /// </summary>
    public class UpdatePurchaseCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public List<PurchaseDetailItem> Items { get; set; } = new();
        public Guid CompanyId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}

