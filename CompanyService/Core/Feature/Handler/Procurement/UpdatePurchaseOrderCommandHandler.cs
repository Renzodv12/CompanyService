using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para actualizar una orden de compra existente
    /// </summary>
    public class UpdatePurchaseOrderCommandHandler : IRequestHandler<UpdatePurchaseOrderCommand, PurchaseOrderResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePurchaseOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PurchaseOrderResponse> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            // Buscar la orden de compra existente
            var purchaseOrder = await _unitOfWork.Repository<PurchaseOrder>()
                .GetByIdAsync(request.Id);

            if (purchaseOrder == null || purchaseOrder.CompanyId != request.CompanyId)
                throw new DefaultException("Orden de compra no encontrada.");

            // Validar que el proveedor existe
            var supplier = await _unitOfWork.Repository<Supplier>()
                .GetByIdAsync(request.Request.SupplierId);

            if (supplier == null)
                throw new DefaultException("Proveedor no encontrado.");

            // Actualizar campos
            purchaseOrder.SupplierId = request.Request.SupplierId;
            purchaseOrder.OrderDate = request.Request.OrderDate;
            purchaseOrder.ExpectedDeliveryDate = request.Request.ExpectedDeliveryDate;
            purchaseOrder.SubTotal = request.Request.SubTotal;
            purchaseOrder.TaxAmount = request.Request.TaxAmount;
            purchaseOrder.DiscountAmount = request.Request.DiscountAmount;
            purchaseOrder.TotalAmount = request.Request.TotalAmount;
            purchaseOrder.Notes = request.Request.Notes;
            purchaseOrder.PaymentTerms = request.Request.PaymentTerms;
            purchaseOrder.DeliveryTerms = request.Request.DeliveryTerms;
            purchaseOrder.ModifiedDate = DateTime.UtcNow;

            // Actualizar items (eliminar existentes y agregar nuevos)
            purchaseOrder.Items.Clear();
            foreach (var itemRequest in request.Request.Items)
            {
                var item = new PurchaseOrderItem
                {
                    Id = Guid.NewGuid(),
                    PurchaseOrderId = purchaseOrder.Id,
                    ProductId = itemRequest.ProductId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = itemRequest.UnitPrice,
                    DiscountAmount = (itemRequest.UnitPrice * itemRequest.Quantity) * (itemRequest.DiscountPercentage / 100),
                    TaxAmount = (itemRequest.UnitPrice * itemRequest.Quantity) * (itemRequest.TaxPercentage / 100),
                    LineTotal = itemRequest.LineTotal,
                    CreatedDate = DateTime.UtcNow
                };
                purchaseOrder.Items.Add(item);
            }

            _unitOfWork.Repository<PurchaseOrder>().Update(purchaseOrder);
            await _unitOfWork.SaveChangesAsync();

            // Mapear a DTO
            return new PurchaseOrderResponse
            {
                Id = purchaseOrder.Id,
                CompanyId = purchaseOrder.CompanyId,
                SupplierId = purchaseOrder.SupplierId,
                SupplierName = supplier.Name,
                OrderNumber = purchaseOrder.OrderNumber,
                OrderDate = purchaseOrder.OrderDate,
                ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
                Status = purchaseOrder.Status,
                SubTotal = purchaseOrder.SubTotal,
                TaxAmount = purchaseOrder.TaxAmount,
                DiscountAmount = purchaseOrder.DiscountAmount,
                TotalAmount = purchaseOrder.TotalAmount,
                Notes = purchaseOrder.Notes,
                PaymentTerms = purchaseOrder.PaymentTerms,
                DeliveryTerms = purchaseOrder.DeliveryTerms,
                CreatedByUserId = purchaseOrder.CreatedBy,
                CreatedAt = purchaseOrder.CreatedAt,
                UpdatedAt = purchaseOrder.ModifiedDate ?? purchaseOrder.CreatedDate,
                Items = new List<PurchaseOrderItemResponse>()
            };
        }
    }
}