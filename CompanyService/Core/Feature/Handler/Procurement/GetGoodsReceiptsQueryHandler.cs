using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Procurement;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using MediatR;
using System.Linq.Expressions;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para obtener recibos de mercancías
    /// </summary>
    public class GetGoodsReceiptsQueryHandler : IRequestHandler<GetGoodsReceiptsQuery, List<GoodsReceiptResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetGoodsReceiptsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<GoodsReceiptResponse>> Handle(GetGoodsReceiptsQuery request, CancellationToken cancellationToken)
        {
            // Construir el predicado de filtros
            Expression<Func<GoodsReceipt, bool>> predicate = gr => gr.CompanyId == request.CompanyId;

            if (request.SupplierId.HasValue)
            {
                var supplierId = request.SupplierId.Value;
                predicate = CombinePredicates(predicate, gr => gr.SupplierId == supplierId);
            }

            if (request.PurchaseOrderId.HasValue)
            {
                var purchaseOrderId = request.PurchaseOrderId.Value;
                predicate = CombinePredicates(predicate, gr => gr.PurchaseOrderId == purchaseOrderId);
            }

            if (request.FromDate.HasValue)
            {
                var fromDate = request.FromDate.Value;
                predicate = CombinePredicates(predicate, gr => gr.ReceiptDate >= fromDate);
            }

            if (request.ToDate.HasValue)
            {
                var toDate = request.ToDate.Value;
                predicate = CombinePredicates(predicate, gr => gr.ReceiptDate <= toDate);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                if (Enum.TryParse<Core.Enums.GoodsReceiptStatus>(request.Status, out var status))
                {
                    predicate = CombinePredicates(predicate, gr => gr.Status == status);
                }
            }

            // Obtener resultados
            var goodsReceipts = await _unitOfWork.Repository<GoodsReceipt>().WhereAsync(predicate);

            // Mapear a DTOs
            return goodsReceipts.Select(gr => new GoodsReceiptResponse
            {
                Id = gr.Id,
                CompanyId = gr.CompanyId,
                CompanyName = "",
                PurchaseOrderId = gr.PurchaseOrderId,
                PurchaseOrderNumber = "",
                SupplierId = gr.SupplierId,
                SupplierName = "",
                ReceiptNumber = gr.ReceiptNumber,
                ReceiptDate = gr.ReceiptDate,
                Status = gr.Status,
                DeliveryNote = gr.DeliveryNote,
                InvoiceNumber = gr.InvoiceNumber,
                TransportCompany = gr.TransportCompany,
                VehicleInfo = gr.VehicleNumber,
                DriverInfo = gr.DriverName,
                FreightCost = 0, // Propiedad no existe en la entidad
                InsuranceCost = 0, // Propiedad no existe en la entidad
                OtherCosts = 0, // Propiedad no existe en la entidad
                TotalCost = 0, // Propiedad no existe en la entidad
                Notes = gr.Notes,
                ReceivedByUserId = gr.ReceivedBy,
                ReceivedByUserName = "",
                InspectedByUserId = gr.InspectedBy,
                InspectedByUserName = "",
                InspectedAt = gr.InspectionDate,
                CreatedAt = gr.CreatedDate,
                UpdatedAt = gr.ModifiedDate ?? gr.CreatedDate,
                Items = new List<GoodsReceiptItemResponse>()
            }).ToList();
        }

        private static Expression<Func<T, bool>> CombinePredicates<T>(
            Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(first.Parameters[0], parameter);
            var left = leftVisitor.Visit(first.Body);
            var rightVisitor = new ReplaceExpressionVisitor(second.Parameters[0], parameter);
            var right = rightVisitor.Visit(second.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }
    }

    internal class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
    }
}