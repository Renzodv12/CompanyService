using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Enums;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para crear una nueva orden de compra
    /// </summary>
    public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, PurchaseOrderResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePurchaseOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PurchaseOrderResponse> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            // Validar que la empresa existe
            var company = await _unitOfWork.Repository<Entities.Company>()
                .GetByIdAsync(request.CompanyId);

            if (company == null)
                throw new DefaultException("Empresa no encontrada.");

            // Validar que el proveedor existe
            var supplier = await _unitOfWork.Repository<Supplier>()
                .GetByIdAsync(request.Request.SupplierId);

            if (supplier == null)
                throw new DefaultException("Proveedor no encontrado.");

            // Crear la orden de compra
            var purchaseOrder = new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                CompanyId = request.CompanyId,
                SupplierId = request.Request.SupplierId,
                OrderNumber = request.Request.OrderNumber,
                OrderDate = request.Request.OrderDate,
                ExpectedDeliveryDate = request.Request.ExpectedDeliveryDate,
                Status = PurchaseOrderStatus.Draft,
                SubTotal = request.Request.SubTotal,
                TaxAmount = request.Request.TaxAmount,
                DiscountAmount = request.Request.DiscountAmount,
                TotalAmount = request.Request.TotalAmount,
                Notes = request.Request.Notes,
                PaymentTerms = request.Request.PaymentTerms,
                DeliveryTerms = request.Request.DeliveryTerms,
                CreatedBy = Guid.Empty, // Se debe obtener del contexto de usuario
                CreatedAt = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            };

            // Agregar items
            foreach (var itemRequest in request.Request.Items)
            {
                var item = new PurchaseOrderItem
                {
                    Id = Guid.NewGuid(),
                    PurchaseOrderId = purchaseOrder.Id,
                    ProductId = itemRequest.ProductId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = itemRequest.UnitPrice,
                    DiscountAmount = itemRequest.DiscountPercentage * itemRequest.UnitPrice * itemRequest.Quantity / 100,
                     TaxAmount = itemRequest.TaxPercentage * itemRequest.UnitPrice * itemRequest.Quantity / 100,
                    LineTotal = itemRequest.LineTotal,
                    Description = itemRequest.Notes
                };
                purchaseOrder.Items.Add(item);
            }

            await _unitOfWork.Repository<PurchaseOrder>().AddAsync(purchaseOrder);
            await _unitOfWork.SaveChangesAsync();

            // Mapear a DTO
            return new PurchaseOrderResponse
            {
                Id = purchaseOrder.Id,
                CompanyId = purchaseOrder.CompanyId,
                CompanyName = company.Name,
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
                CreatedByUserId = Guid.Empty, // Se debe obtener del contexto de usuario
                CreatedAt = purchaseOrder.CreatedAt,
                UpdatedAt = purchaseOrder.ModifiedDate ?? purchaseOrder.CreatedDate,
                Items = purchaseOrder.Items.Select(item => new PurchaseOrderItemResponse
                {
                    Id = item.Id,
                    PurchaseOrderId = item.PurchaseOrderId,
                    ProductId = item.ProductId,
                    ProductName = "", // Se llenará desde el producto
                    ProductSku = "", // Se llenará desde el producto
                    Quantity = item.Quantity,
                    ReceivedQuantity = item.ReceivedQuantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercentage = 0, // No está en la entidad
                    TaxPercentage = 0, // No está en la entidad
                    LineTotal = item.LineTotal,
                    Notes = item.Description,
                    CreatedAt = item.CreatedDate,
                    UpdatedAt = item.ModifiedDate ?? item.CreatedDate
                }).ToList()
            };
        }
    }
}