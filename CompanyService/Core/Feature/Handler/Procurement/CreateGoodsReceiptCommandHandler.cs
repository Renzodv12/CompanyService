using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para crear recibos de mercancías
    /// </summary>
    public class CreateGoodsReceiptCommandHandler : IRequestHandler<CreateGoodsReceiptCommand, GoodsReceiptResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateGoodsReceiptCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GoodsReceiptResponse> Handle(CreateGoodsReceiptCommand request, CancellationToken cancellationToken)
        {
            // Validar que la orden de compra existe
            var purchaseOrder = await _unitOfWork.Repository<PurchaseOrder>()
                .FirstOrDefaultAsync(po => po.Id == request.Request.PurchaseOrderId);

            if (purchaseOrder == null || purchaseOrder.CompanyId != request.CompanyId)
                throw new DefaultException("Orden de compra no encontrada.");

            // Crear el recibo de mercancías
            var goodsReceipt = new GoodsReceipt
            {
                CompanyId = request.CompanyId,
                PurchaseOrderId = request.Request.PurchaseOrderId,
                SupplierId = request.Request.SupplierId,
                ReceiptNumber = request.Request.ReceiptNumber,
                ReceiptDate = request.Request.ReceiptDate,
                Status = Core.Enums.GoodsReceiptStatus.Pending,
                DeliveryNote = request.Request.DeliveryNote,
                InvoiceNumber = request.Request.InvoiceNumber,
                TransportCompany = request.Request.TransportCompany,
                VehicleNumber = request.Request.VehicleInfo,
                DriverName = request.Request.DriverInfo,
                Notes = request.Request.Notes,
                ReceivedBy = request.Request.ReceivedByUserId ?? Guid.Empty,
                CreatedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            // Crear los items del recibo
            foreach (var itemRequest in request.Request.Items)
            {
                var item = new GoodsReceiptItem
                {
                    GoodsReceiptId = goodsReceipt.Id,
                    PurchaseOrderItemId = itemRequest.PurchaseOrderItemId,
                    ProductId = itemRequest.ProductId,
                    OrderedQuantity = itemRequest.OrderedQuantity,
                    ReceivedQuantity = itemRequest.ReceivedQuantity,
                    AcceptedQuantity = itemRequest.AcceptedQuantity,
                    RejectedQuantity = itemRequest.RejectedQuantity,
                    DamagedQuantity = itemRequest.DamagedQuantity,
                    QualityStatus = itemRequest.QualityStatus,
                    QualityNotes = itemRequest.QualityNotes,
                    BatchNumber = itemRequest.BatchNumber,
                    SerialNumber = itemRequest.SerialNumbers,
                    ExpiryDate = itemRequest.ExpirationDate,
                    ManufactureDate = itemRequest.ManufactureDate,
                    StorageLocation = itemRequest.StorageLocation,
                    WarehouseId = itemRequest.WarehouseId,
                    RequiresInspection = itemRequest.RequiresInspection,
                    CreatedDate = DateTime.UtcNow
                };
                
                goodsReceipt.Items.Add(item);
            }

            await _unitOfWork.Repository<GoodsReceipt>().AddAsync(goodsReceipt);
            await _unitOfWork.SaveChangesAsync();

            // Mapear a response (sin cargar relaciones para simplificar)
            return new GoodsReceiptResponse
            {
                Id = goodsReceipt.Id,
                CompanyId = goodsReceipt.CompanyId,
                CompanyName = "",
                PurchaseOrderId = goodsReceipt.PurchaseOrderId,
                PurchaseOrderNumber = "",
                SupplierId = goodsReceipt.SupplierId,
                SupplierName = "",
                ReceiptNumber = goodsReceipt.ReceiptNumber,
                ReceiptDate = goodsReceipt.ReceiptDate,
                Status = goodsReceipt.Status,
                DeliveryNote = goodsReceipt.DeliveryNote,
                InvoiceNumber = goodsReceipt.InvoiceNumber,
                TransportCompany = goodsReceipt.TransportCompany,
                VehicleInfo = goodsReceipt.VehicleNumber,
                DriverInfo = goodsReceipt.DriverName,
                Notes = goodsReceipt.Notes,
                ReceivedByUserId = goodsReceipt.ReceivedBy,
                ReceivedByUserName = "",
                CreatedAt = goodsReceipt.CreatedAt,
                UpdatedAt = goodsReceipt.ModifiedDate ?? goodsReceipt.CreatedAt,
                Items = goodsReceipt.Items.Select(item => new GoodsReceiptItemResponse
                {
                    Id = item.Id,
                    GoodsReceiptId = item.GoodsReceiptId,
                    PurchaseOrderItemId = item.PurchaseOrderItemId,
                    ProductId = item.ProductId,
                    ProductName = "",
                    ProductSku = "",
                    WarehouseId = item.WarehouseId,
                    WarehouseName = "",
                    OrderedQuantity = item.OrderedQuantity,
                    ReceivedQuantity = item.ReceivedQuantity,
                    AcceptedQuantity = item.AcceptedQuantity,
                    RejectedQuantity = item.RejectedQuantity,
                    DamagedQuantity = item.DamagedQuantity,
                    QualityStatus = item.QualityStatus,
                    BatchNumber = item.BatchNumber,
                    SerialNumbers = item.SerialNumber,
                    ExpirationDate = item.ExpiryDate,
                    ManufactureDate = item.ManufactureDate,
                    StorageLocation = item.StorageLocation,
                    RequiresInspection = item.RequiresInspection,
                    InspectedByUserId = item.InspectedBy,
                    InspectedByUserName = "",
                    InspectedAt = item.InspectionDate,
                    CreatedAt = item.CreatedDate,
                    UpdatedAt = item.ModifiedDate ?? item.CreatedDate
                }).ToList()
            };
        }
    }
}