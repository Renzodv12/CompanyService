using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CompanyService.WebApi.Endpoints
{
    public static class ProcurementEndpoints
    {
        public static void MapProcurementEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/companies/{companyId:guid}/procurement")
                .WithTags("Procurement")
                .RequireAuthorization();

            // Purchase Order endpoints
            group.MapGet("/purchase-orders", GetPurchaseOrders)
                .WithName("GetPurchaseOrders")
                .WithOpenApi();

            group.MapGet("/purchase-orders/{id:guid}", GetPurchaseOrderById)
                .WithName("GetPurchaseOrderById")
                .WithOpenApi();

            group.MapPost("/purchase-orders", CreatePurchaseOrder)
                .WithName("CreatePurchaseOrder")
                .WithOpenApi();

            group.MapPut("/purchase-orders/{id:guid}", UpdatePurchaseOrder)
                .WithName("UpdatePurchaseOrder")
                .WithOpenApi();

            group.MapPatch("/purchase-orders/{id:guid}/status", UpdatePurchaseOrderStatus)
                .WithName("UpdatePurchaseOrderStatus")
                .WithOpenApi();

            group.MapPost("/purchase-orders/{id:guid}/approve", ApprovePurchaseOrder)
                .WithName("ApprovePurchaseOrder")
                .WithOpenApi();

            group.MapPost("/purchase-orders/{id:guid}/reject", RejectPurchaseOrder)
                .WithName("RejectPurchaseOrder")
                .WithOpenApi();

            group.MapPost("/purchase-orders/{id:guid}/cancel", CancelPurchaseOrder)
                .WithName("CancelPurchaseOrder")
                .WithOpenApi();

            group.MapDelete("/purchase-orders/{id:guid}", DeletePurchaseOrder)
                .WithName("DeletePurchaseOrder")
                .WithOpenApi();

            // Purchase Order Items endpoints
            group.MapGet("/purchase-orders/{purchaseOrderId:guid}/items", GetPurchaseOrderItems)
                .WithName("GetPurchaseOrderItems")
                .WithOpenApi();

            group.MapPost("/purchase-orders/{purchaseOrderId:guid}/items", AddPurchaseOrderItem)
                .WithName("AddPurchaseOrderItem")
                .WithOpenApi();

            group.MapPut("/purchase-orders/{purchaseOrderId:guid}/items/{itemId:guid}", UpdatePurchaseOrderItem)
                .WithName("UpdatePurchaseOrderItem")
                .WithOpenApi();

            group.MapDelete("/purchase-orders/{purchaseOrderId:guid}/items/{itemId:guid}", DeletePurchaseOrderItem)
                .WithName("DeletePurchaseOrderItem")
                .WithOpenApi();

            // Quotation endpoints
            group.MapGet("/quotations", GetQuotations)
                .WithName("GetQuotations")
                .WithOpenApi();

            group.MapGet("/quotations/{id:guid}", GetQuotationById)
                .WithName("GetQuotationById")
                .WithOpenApi();

            group.MapPost("/quotations", CreateQuotation)
                .WithName("CreateQuotation")
                .WithOpenApi();

            group.MapPut("/quotations/{id:guid}", UpdateQuotation)
                .WithName("UpdateQuotation")
                .WithOpenApi();

            group.MapPost("/quotations/{id:guid}/accept", AcceptQuotation)
                .WithName("AcceptQuotation")
                .WithOpenApi();

            group.MapPost("/quotations/{id:guid}/reject", RejectQuotation)
                .WithName("RejectQuotation")
                .WithOpenApi();

            group.MapPost("/quotations/{id:guid}/convert-to-purchase-order", ConvertQuotationToPurchaseOrder)
                .WithName("ConvertQuotationToPurchaseOrder")
                .WithOpenApi();

            group.MapDelete("/quotations/{id:guid}", DeleteQuotation)
                .WithName("DeleteQuotation")
                .WithOpenApi();

            // Goods Receipt endpoints
            group.MapGet("/goods-receipts", GetGoodsReceipts)
                .WithName("GetGoodsReceipts")
                .WithOpenApi();

            group.MapGet("/goods-receipts/{id:guid}", GetGoodsReceiptById)
                .WithName("GetGoodsReceiptById")
                .WithOpenApi();

            group.MapPost("/goods-receipts", CreateGoodsReceipt)
                .WithName("CreateGoodsReceipt")
                .WithOpenApi();

            group.MapPut("/goods-receipts/{id:guid}", UpdateGoodsReceipt)
                .WithName("UpdateGoodsReceipt")
                .WithOpenApi();

            group.MapDelete("/goods-receipts/{id:guid}", DeleteGoodsReceipt)
                .WithName("DeleteGoodsReceipt")
                .WithOpenApi();
        }

        #region Purchase Orders

        private static async Task<IResult> GetPurchaseOrders(
            Guid companyId,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var purchaseOrders = await procurementService.GetPurchaseOrdersByCompanyAsync(companyId);
                return Results.Ok(purchaseOrders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener órdenes de compra para la empresa {CompanyId}", companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> GetPurchaseOrderById(
            Guid companyId,
            Guid id,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var purchaseOrder = await procurementService.GetPurchaseOrderByIdAsync(id);
                if (purchaseOrder == null)
                    return Results.NotFound($"Orden de compra con ID {id} no encontrada");

                return Results.Ok(purchaseOrder);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener orden de compra {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> CreatePurchaseOrder(
            Guid companyId,
            PurchaseOrder purchaseOrder,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                purchaseOrder.CompanyId = companyId;
                var createdOrder = await procurementService.CreatePurchaseOrderAsync(purchaseOrder);
                return Results.Created($"/api/companies/{companyId}/procurement/purchase-orders/{createdOrder.Id}", createdOrder);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear orden de compra para la empresa {CompanyId}", companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> UpdatePurchaseOrder(
            Guid companyId,
            Guid id,
            UpdatePurchaseOrderRequest request,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var updatedOrder = await procurementService.UpdatePurchaseOrderAsync(id, request);
                if (updatedOrder == null)
                    return Results.NotFound($"Orden de compra con ID {id} no encontrada");

                return Results.Ok(updatedOrder);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar orden de compra {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> UpdatePurchaseOrderStatus(
            Guid companyId,
            Guid id,
            UpdatePurchaseOrderStatusRequest request,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var updatedOrder = await procurementService.UpdatePurchaseOrderStatusAsync(id, request);
                if (updatedOrder == null)
                    return Results.NotFound($"Orden de compra con ID {id} no encontrada");

                return Results.Ok(updatedOrder);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar estado de orden de compra {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> ApprovePurchaseOrder(
            Guid companyId,
            Guid id,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var userId = GetUserId(user);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var approvedOrder = await procurementService.ApprovePurchaseOrderAsync(id, userId);
                return Results.Ok(approvedOrder);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al aprobar orden de compra {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> RejectPurchaseOrder(
            Guid companyId,
            Guid id,
            string reason,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var userId = GetUserId(user);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var rejectedOrder = await procurementService.RejectPurchaseOrderAsync(id, userId, reason);
                return Results.Ok(rejectedOrder);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al rechazar orden de compra {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> CancelPurchaseOrder(
            Guid companyId,
            Guid id,
            string reason,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var cancelledOrder = await procurementService.CancelPurchaseOrderAsync(id, reason);
                return Results.Ok(cancelledOrder);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al cancelar orden de compra {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> DeletePurchaseOrder(
            Guid companyId,
            Guid id,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var deleted = await procurementService.DeletePurchaseOrderAsync(id);
                if (!deleted)
                    return Results.NotFound($"Orden de compra con ID {id} no encontrada");

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al eliminar orden de compra {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        #endregion

        #region Purchase Order Items

        private static async Task<IResult> GetPurchaseOrderItems(
            Guid companyId,
            Guid purchaseOrderId,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var items = await procurementService.GetPurchaseOrderItemsAsync(purchaseOrderId);
                return Results.Ok(items);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener items de orden de compra {PurchaseOrderId} para la empresa {CompanyId}", purchaseOrderId, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> AddPurchaseOrderItem(
            Guid companyId,
            Guid purchaseOrderId,
            CreatePurchaseOrderItemRequest request,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var item = await procurementService.AddPurchaseOrderItemAsync(purchaseOrderId, request);
                return Results.Ok(item);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al agregar item a orden de compra {PurchaseOrderId} para la empresa {CompanyId}", purchaseOrderId, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> UpdatePurchaseOrderItem(
            Guid companyId,
            Guid purchaseOrderId,
            Guid itemId,
            UpdatePurchaseOrderItemRequest request,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var updatedItem = await procurementService.UpdatePurchaseOrderItemAsync(purchaseOrderId, itemId, request);
                if (updatedItem == null)
                    return Results.NotFound($"Item con ID {itemId} no encontrado");

                return Results.Ok(updatedItem);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar item {ItemId} de orden de compra {PurchaseOrderId} para la empresa {CompanyId}", itemId, purchaseOrderId, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> DeletePurchaseOrderItem(
            Guid companyId,
            Guid purchaseOrderId,
            Guid itemId,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                await procurementService.RemovePurchaseOrderItemAsync(purchaseOrderId, itemId);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al eliminar item {ItemId} de orden de compra {PurchaseOrderId} para la empresa {CompanyId}", itemId, purchaseOrderId, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        #endregion

        #region Quotations

        private static async Task<IResult> GetQuotations(
            Guid companyId,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var quotations = await procurementService.GetQuotationsByCompanyAsync(companyId);
                return Results.Ok(quotations);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener cotizaciones para la empresa {CompanyId}", companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> GetQuotationById(
            Guid companyId,
            Guid id,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var quotation = await procurementService.GetQuotationByIdAsync(id);
                if (quotation == null)
                    return Results.NotFound($"Cotización con ID {id} no encontrada");

                return Results.Ok(quotation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener cotización {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> CreateQuotation(
            Guid companyId,
            CreateQuotationRequest request,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var quotation = new Quotation
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId,
                    SupplierId = request.SupplierId,
                    QuotationNumber = request.QuotationNumber,
                    QuotationDate = request.RequestDate,
                    ValidUntil = request.ValidUntil,
                    Status = QuotationStatus.Draft,
                    SubTotal = request.SubTotal,
                    TaxAmount = request.TaxAmount,
                    DiscountAmount = request.DiscountAmount,
                    TotalAmount = request.TotalAmount,
                    Notes = request.Notes,
                    PaymentTerms = request.PaymentTerms,
                    DeliveryTerms = request.DeliveryTerms,
                    RequestedBy = GetUserId(user),
                    CreatedDate = DateTime.UtcNow,
                    Items = request.Items.Select(item => new QuotationItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        DiscountAmount = item.Quantity * item.UnitPrice * (item.DiscountPercentage / 100),
                        TaxAmount = item.Quantity * item.UnitPrice * (item.TaxPercentage / 100),
                        LineTotal = item.LineTotal,
                        Specifications = item.Specifications,
                        LeadTimeDays = item.LeadTimeDays,
                        CreatedDate = DateTime.UtcNow
                    }).ToList()
                };
                
                var createdQuotation = await procurementService.CreateQuotationAsync(quotation);
                return Results.Created($"/api/companies/{companyId}/procurement/quotations/{createdQuotation.Id}", createdQuotation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear cotización para la empresa {CompanyId}", companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> UpdateQuotation(
            Guid companyId,
            Guid id,
            UpdateQuotationRequest request,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var existingQuotation = await procurementService.GetQuotationByIdAsync(id);
                if (existingQuotation == null)
                    return Results.NotFound($"Cotización con ID {id} no encontrada");

                existingQuotation.SupplierId = request.SupplierId;
                existingQuotation.QuotationDate = request.RequestDate;
                existingQuotation.ValidUntil = request.ValidUntil;
                existingQuotation.SubTotal = request.SubTotal;
                existingQuotation.TaxAmount = request.TaxAmount;
                existingQuotation.DiscountAmount = request.DiscountAmount;
                existingQuotation.TotalAmount = request.TotalAmount;
                existingQuotation.Notes = request.Notes;
                existingQuotation.PaymentTerms = request.PaymentTerms;
                existingQuotation.DeliveryTerms = request.DeliveryTerms;
                existingQuotation.ModifiedDate = DateTime.UtcNow;
                
                // Update items
                existingQuotation.Items.Clear();
                existingQuotation.Items = request.Items.Select(item => new QuotationItem
                {
                    Id = item.Id ?? Guid.NewGuid(),
                    QuotationId = id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountAmount = item.Quantity * item.UnitPrice * (item.DiscountPercentage / 100),
                    TaxAmount = item.Quantity * item.UnitPrice * (item.TaxPercentage / 100),
                    LineTotal = item.LineTotal,
                    Specifications = item.Specifications,
                    LeadTimeDays = item.LeadTimeDays,
                    CreatedDate = item.Id.HasValue ? existingQuotation.CreatedDate : DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                }).ToList();

                var updatedQuotation = await procurementService.UpdateQuotationAsync(id, existingQuotation);
                if (updatedQuotation == null)
                    return Results.NotFound($"Cotización con ID {id} no encontrada");

                return Results.Ok(updatedQuotation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar cotización {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> AcceptQuotation(
            Guid companyId,
            Guid id,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var acceptedQuotation = await procurementService.AcceptQuotationAsync(id);
                return Results.Ok(acceptedQuotation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al aceptar cotización {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> RejectQuotation(
            Guid companyId,
            Guid id,
            string reason,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var rejectedQuotation = await procurementService.RejectQuotationAsync(id, reason);
                return Results.Ok(rejectedQuotation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al rechazar cotización {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> ConvertQuotationToPurchaseOrder(
            Guid companyId,
            Guid id,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var userId = GetUserId(user);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var purchaseOrder = await procurementService.ConvertQuotationToPurchaseOrderAsync(id, userId);
                return Results.Ok(purchaseOrder);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al convertir cotización {Id} a orden de compra para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> DeleteQuotation(
            Guid companyId,
            Guid id,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var deleted = await procurementService.DeleteQuotationAsync(id);
                if (!deleted)
                    return Results.NotFound($"Cotización con ID {id} no encontrada");

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al eliminar cotización {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        #endregion

        #region Goods Receipts

        private static async Task<IResult> GetGoodsReceipts(
            Guid companyId,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var goodsReceipts = await procurementService.GetGoodsReceiptsByCompanyAsync(companyId);
                return Results.Ok(goodsReceipts);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener recibos de mercancía para la empresa {CompanyId}", companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> GetGoodsReceiptById(
            Guid companyId,
            Guid id,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var goodsReceipt = await procurementService.GetGoodsReceiptByIdAsync(id);
                if (goodsReceipt == null)
                    return Results.NotFound($"Recibo de mercancía con ID {id} no encontrado");

                return Results.Ok(goodsReceipt);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener recibo de mercancía {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> CreateGoodsReceipt(
            Guid companyId,
            CreateGoodsReceiptRequest request,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var goodsReceipt = new GoodsReceipt
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId,
                    PurchaseOrderId = request.PurchaseOrderId,
                    SupplierId = request.SupplierId,
                    ReceiptNumber = request.ReceiptNumber,
                    ReceiptDate = request.ReceiptDate,
                    Status = GoodsReceiptStatus.Draft,
                    DeliveryNote = request.DeliveryNote,
                    InvoiceNumber = request.InvoiceNumber,
                    TransportCompany = request.TransportCompany,
                    VehicleNumber = request.VehicleInfo,
                    DriverName = request.DriverInfo,
                    Notes = request.Notes,
                    ReceivedBy = GetUserId(user),
                    CreatedDate = DateTime.UtcNow,
                    Items = request.Items.Select(item => new GoodsReceiptItem
                    {
                        Id = Guid.NewGuid(),
                        PurchaseOrderItemId = item.PurchaseOrderItemId,
                        ProductId = item.ProductId,
                        OrderedQuantity = item.OrderedQuantity,
                        ReceivedQuantity = item.ReceivedQuantity,
                        AcceptedQuantity = item.AcceptedQuantity,
                        RejectedQuantity = item.RejectedQuantity,
                        DamagedQuantity = item.DamagedQuantity,
                        QualityStatus = item.QualityStatus,
                        QualityNotes = item.QualityNotes,
                        BatchNumber = item.BatchNumber,
                        SerialNumber = item.SerialNumbers,
                        ExpiryDate = item.ExpirationDate,
                        ManufactureDate = item.ManufactureDate,
                        StorageLocation = item.StorageLocation,
                        CreatedDate = DateTime.UtcNow
                    }).ToList()
                };
                
                var createdGoodsReceipt = await procurementService.CreateGoodsReceiptAsync(goodsReceipt);
                return Results.Created($"/api/companies/{companyId}/procurement/goods-receipts/{createdGoodsReceipt.Id}", createdGoodsReceipt);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear recibo de mercancía para la empresa {CompanyId}", companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> UpdateGoodsReceipt(
            Guid companyId,
            Guid id,
            UpdateGoodsReceiptRequest request,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var existingGoodsReceipt = await procurementService.GetGoodsReceiptByIdAsync(id);
                if (existingGoodsReceipt == null)
                    return Results.NotFound($"Recibo de mercancía con ID {id} no encontrado");

                existingGoodsReceipt.ReceiptDate = request.ReceiptDate;
                existingGoodsReceipt.DeliveryNote = request.DeliveryNote;
                existingGoodsReceipt.InvoiceNumber = request.InvoiceNumber;
                existingGoodsReceipt.TransportCompany = request.TransportCompany;
                existingGoodsReceipt.VehicleNumber = request.VehicleInfo;
                existingGoodsReceipt.DriverName = request.DriverInfo;
                existingGoodsReceipt.Notes = request.Notes;
                existingGoodsReceipt.ModifiedDate = DateTime.UtcNow;
                
                // Update items
                existingGoodsReceipt.Items.Clear();
                existingGoodsReceipt.Items = request.Items.Select(item => new GoodsReceiptItem
                {
                    Id = item.Id ?? Guid.NewGuid(),
                    GoodsReceiptId = id,
                    PurchaseOrderItemId = item.PurchaseOrderItemId,
                    ProductId = item.ProductId,
                    ReceivedQuantity = item.ReceivedQuantity,
                    AcceptedQuantity = item.AcceptedQuantity,
                    RejectedQuantity = item.RejectedQuantity,
                    DamagedQuantity = item.DamagedQuantity,
                    QualityStatus = item.QualityStatus,
                    QualityNotes = item.Notes,
                    BatchNumber = item.BatchNumber,
                    SerialNumber = item.SerialNumbers,
                ExpiryDate = item.ExpirationDate,
                    ManufactureDate = item.ManufactureDate,
                    StorageLocation = item.StorageLocation,
                    CreatedDate = item.Id.HasValue ? existingGoodsReceipt.CreatedDate : DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                }).ToList();

                var updatedGoodsReceipt = await procurementService.UpdateGoodsReceiptAsync(existingGoodsReceipt);
                if (updatedGoodsReceipt == null)
                    return Results.NotFound($"Recibo de mercancía con ID {id} no encontrado");

                return Results.Ok(updatedGoodsReceipt);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar recibo de mercancía {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        private static async Task<IResult> DeleteGoodsReceipt(
            Guid companyId,
            Guid id,
            ClaimsPrincipal user,
            IProcurementService procurementService,
            ILogger<IProcurementService> logger)
        {
            try
            {
                var deleted = await procurementService.DeleteGoodsReceiptAsync(id);
                if (!deleted)
                    return Results.NotFound($"Recibo de mercancía con ID {id} no encontrado");

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al eliminar recibo de mercancía {Id} para la empresa {CompanyId}", id, companyId);
                return Results.Problem("Error interno del servidor");
            }
        }

        #endregion

        #region Helper Methods

        private static Guid GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        #endregion
    }
}