using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Procurement;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para obtener una orden de compra por ID
    /// </summary>
    public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderResponse?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPurchaseOrderByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PurchaseOrderResponse?> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var purchaseOrders = await _unitOfWork.Repository<PurchaseOrder>()
                .GetAllAsync();

            var purchaseOrder = purchaseOrders.FirstOrDefault(po => 
                po.Id == request.Id && po.CompanyId == request.CompanyId);

            if (purchaseOrder == null)
                return null;

            return new PurchaseOrderResponse
            {
                Id = purchaseOrder.Id,
                CompanyId = purchaseOrder.CompanyId,
                CompanyName = purchaseOrder.Company?.Name ?? "",
                SupplierId = purchaseOrder.SupplierId,
                SupplierName = purchaseOrder.Supplier?.Name ?? "",
                OrderNumber = purchaseOrder.OrderNumber,
                OrderDate = purchaseOrder.OrderDate,
                ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
                ActualDeliveryDate = purchaseOrder.ActualDeliveryDate,
                Status = purchaseOrder.Status,
                SubTotal = purchaseOrder.SubTotal,
                TaxAmount = purchaseOrder.TaxAmount,
                DiscountAmount = purchaseOrder.DiscountAmount,
                TotalAmount = purchaseOrder.TotalAmount,
                Notes = purchaseOrder.Notes,
                PaymentTerms = purchaseOrder.PaymentTerms,
                DeliveryTerms = purchaseOrder.DeliveryTerms,
                CreatedByUserId = purchaseOrder.CreatedBy,
                CreatedByUserName = purchaseOrder.CreatedByUser?.Name ?? "",
                ApprovedByUserId = purchaseOrder.ApprovedBy,
                ApprovedByUserName = purchaseOrder.ApprovedByUser?.Name ?? "",
                ApprovedAt = purchaseOrder.ApprovedDate,
                CreatedAt = purchaseOrder.CreatedAt,
                UpdatedAt = purchaseOrder.ModifiedDate ?? purchaseOrder.CreatedDate,
                Items = purchaseOrder.Items?.Select(item => new PurchaseOrderItemResponse
                {
                    Id = item.Id,
                    PurchaseOrderId = item.PurchaseOrderId,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? "",
                    ProductSku = item.Product?.SKU ?? "",
                    Quantity = item.Quantity,
                    ReceivedQuantity = item.ReceivedQuantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercentage = item.DiscountAmount / (item.UnitPrice * item.Quantity) * 100,
                    TaxPercentage = item.TaxAmount / (item.UnitPrice * item.Quantity) * 100,
                    LineTotal = item.LineTotal,
                    Notes = item.Description,
                    CreatedAt = item.CreatedDate,
                    UpdatedAt = item.ModifiedDate ?? item.CreatedDate
                }).ToList() ?? new List<PurchaseOrderItemResponse>()
            };
        }
    }
}