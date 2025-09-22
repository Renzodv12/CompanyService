using MediatR;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    /// <summary>
    /// Command para crear un recibo de mercancías
    /// </summary>
    public class CreateGoodsReceiptCommand : IRequest<GoodsReceiptResponse>
    {
        public Guid CompanyId { get; set; }
        public CreateGoodsReceiptRequest Request { get; set; } = new();
    }
}