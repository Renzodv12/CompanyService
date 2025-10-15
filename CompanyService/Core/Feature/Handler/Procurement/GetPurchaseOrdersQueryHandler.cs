using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Procurement;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para obtener lista de órdenes de compra con filtros
    /// </summary>
    public class GetPurchaseOrdersQueryHandler : IRequestHandler<GetPurchaseOrdersQuery, List<PurchaseOrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPurchaseOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PurchaseOrderResponse>> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
        {
            var purchaseOrders = await _unitOfWork.Repository<PurchaseOrder>()
                .GetAllAsync();
            
            var filteredPurchaseOrders = purchaseOrders.Where(po => po.CompanyId == request.CompanyId);

            // Aplicar filtros si existen
            if (request.Filter != null)
            {
                if (request.Filter.SupplierId.HasValue)
                    filteredPurchaseOrders = filteredPurchaseOrders.Where(po => po.SupplierId == request.Filter.SupplierId.Value);

                if (request.Filter.Status.HasValue)
                    filteredPurchaseOrders = filteredPurchaseOrders.Where(po => po.Status == request.Filter.Status.Value);

                if (request.Filter.StartDate.HasValue)
                    filteredPurchaseOrders = filteredPurchaseOrders.Where(po => po.OrderDate >= request.Filter.StartDate.Value);

                if (request.Filter.EndDate.HasValue)
                    filteredPurchaseOrders = filteredPurchaseOrders.Where(po => po.OrderDate <= request.Filter.EndDate.Value);

                if (!string.IsNullOrEmpty(request.Filter.OrderNumber))
                    filteredPurchaseOrders = filteredPurchaseOrders.Where(po => po.OrderNumber.Contains(request.Filter.OrderNumber));

                if (!string.IsNullOrEmpty(request.Filter.SearchTerm))
                {
                    var searchTerm = request.Filter.SearchTerm.ToLower();
                    filteredPurchaseOrders = filteredPurchaseOrders.Where(po => 
                        po.OrderNumber.ToLower().Contains(searchTerm) ||
                        (po.Supplier?.Name?.ToLower().Contains(searchTerm) ?? false) ||
                        (po.Notes?.ToLower().Contains(searchTerm) ?? false));
                }
            }

            var purchaseOrdersList = filteredPurchaseOrders.ToList();

            return purchaseOrdersList.Select(purchaseOrder => new PurchaseOrderResponse
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
                CreatedByUserName = purchaseOrder.CreatedByUser != null ? $"{purchaseOrder.CreatedByUser.FirstName} {purchaseOrder.CreatedByUser.LastName}".Trim() : "",
                ApprovedByUserId = purchaseOrder.ApprovedBy,
                ApprovedByUserName = purchaseOrder.ApprovedByUser != null ? $"{purchaseOrder.ApprovedByUser.FirstName} {purchaseOrder.ApprovedByUser.LastName}".Trim() : "",
                ApprovedAt = purchaseOrder.ApprovedDate,
                CreatedAt = purchaseOrder.CreatedAt,
                UpdatedAt = purchaseOrder.ModifiedDate ?? purchaseOrder.CreatedDate,
                Items = new List<PurchaseOrderItemResponse>() // Lista vacía para la vista de lista
            }).ToList();
        }
    }
}