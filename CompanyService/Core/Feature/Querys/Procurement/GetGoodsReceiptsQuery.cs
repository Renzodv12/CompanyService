using MediatR;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Querys.Procurement
{
    /// <summary>
    /// Query para obtener recibos de mercancías
    /// </summary>
    public class GetGoodsReceiptsQuery : IRequest<List<GoodsReceiptResponse>>
    {
        public Guid CompanyId { get; set; }
        public Guid? SupplierId { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}