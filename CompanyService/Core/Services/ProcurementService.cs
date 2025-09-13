using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using CompanyService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyService.Core.Services
{
    public class ProcurementService : IProcurementService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProcurementService> _logger;

        public ProcurementService(ApplicationDbContext context, ILogger<ProcurementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Purchase Order Management

        public async Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            try
            {
                purchaseOrder.Id = Guid.NewGuid();
                purchaseOrder.CreatedDate = DateTime.UtcNow;
                purchaseOrder.Status = PurchaseOrderStatus.Draft;

                _context.PurchaseOrders.Add(purchaseOrder);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Purchase order {OrderNumber} created successfully", purchaseOrder.OrderNumber);
                return purchaseOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating purchase order {OrderNumber}", purchaseOrder.OrderNumber);
                throw;
            }
        }

        public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(Guid id)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Items)
                    .ThenInclude(i => i.Product)
                .Include(po => po.Company)
                .Include(po => po.Supplier)
                .Include(po => po.CreatedByUser)
                .Include(po => po.GoodsReceipts)
                .FirstOrDefaultAsync(po => po.Id == id);
        }

        public async Task<PurchaseOrder?> GetPurchaseOrderByNumberAsync(string orderNumber)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Items)
                    .ThenInclude(i => i.Product)
                .Include(po => po.Company)
                .Include(po => po.Supplier)
                .FirstOrDefaultAsync(po => po.OrderNumber == orderNumber);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByCompanyAsync(Guid companyId)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.CreatedByUser)
                .Where(po => po.CompanyId == companyId)
                .OrderByDescending(po => po.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(Guid companyId, PurchaseOrderStatus status)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.CreatedByUser)
                .Where(po => po.CompanyId == companyId && po.Status == status)
                .OrderByDescending(po => po.CreatedDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            try
            {
                purchaseOrder.ModifiedDate = DateTime.UtcNow;
                _context.PurchaseOrders.Update(purchaseOrder);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Purchase order {OrderNumber} updated successfully", purchaseOrder.OrderNumber);
                return purchaseOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating purchase order {OrderNumber}", purchaseOrder.OrderNumber);
                throw;
            }
        }

        public async Task<bool> DeletePurchaseOrderAsync(Guid id)
        {
            try
            {
                var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
                if (purchaseOrder == null) return false;

                _context.PurchaseOrders.Remove(purchaseOrder);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Purchase order {Id} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting purchase order {Id}", id);
                throw;
            }
        }

        public async Task<PurchaseOrder> ApprovePurchaseOrderAsync(Guid id, Guid approvedBy)
        {
            var purchaseOrder = await GetPurchaseOrderByIdAsync(id);
            if (purchaseOrder == null)
                throw new ArgumentException("Purchase order not found");

            purchaseOrder.Status = PurchaseOrderStatus.Approved;
            purchaseOrder.ApprovedBy = approvedBy;
            purchaseOrder.ApprovedDate = DateTime.UtcNow;
            purchaseOrder.ModifiedDate = DateTime.UtcNow;

            return await UpdatePurchaseOrderAsync(purchaseOrder);
        }

        public async Task<PurchaseOrder> RejectPurchaseOrderAsync(Guid id, Guid rejectedBy, string reason)
        {
            var purchaseOrder = await GetPurchaseOrderByIdAsync(id);
            if (purchaseOrder == null)
                throw new ArgumentException("Purchase order not found");

            purchaseOrder.Status = PurchaseOrderStatus.Rejected;
            purchaseOrder.RejectedBy = rejectedBy;
            purchaseOrder.RejectedDate = DateTime.UtcNow;
            purchaseOrder.RejectionReason = reason;
            purchaseOrder.ModifiedDate = DateTime.UtcNow;

            return await UpdatePurchaseOrderAsync(purchaseOrder);
        }

        public async Task<PurchaseOrder> SendPurchaseOrderAsync(Guid id)
        {
            var purchaseOrder = await GetPurchaseOrderByIdAsync(id);
            if (purchaseOrder == null)
                throw new ArgumentException("Purchase order not found");

            purchaseOrder.Status = PurchaseOrderStatus.Sent;
            purchaseOrder.SentDate = DateTime.UtcNow;
            purchaseOrder.ModifiedDate = DateTime.UtcNow;

            return await UpdatePurchaseOrderAsync(purchaseOrder);
        }

        public async Task<PurchaseOrder> CancelPurchaseOrderAsync(Guid id, string reason)
        {
            var purchaseOrder = await GetPurchaseOrderByIdAsync(id);
            if (purchaseOrder == null)
                throw new ArgumentException("Purchase order not found");

            purchaseOrder.Status = PurchaseOrderStatus.Cancelled;
            purchaseOrder.CancelledDate = DateTime.UtcNow;
            purchaseOrder.CancellationReason = reason;
            purchaseOrder.ModifiedDate = DateTime.UtcNow;

            return await UpdatePurchaseOrderAsync(purchaseOrder);
        }

        public async Task<IEnumerable<GoodsReceipt>> GetGoodsReceiptsAsync(Guid companyId)
        {
            try
            {
                _logger.LogInformation("Getting goods receipts for company {CompanyId}", companyId);
                
                // TODO: Implement actual goods receipt retrieval logic
                throw new NotImplementedException("GetGoodsReceiptsAsync not implemented");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting goods receipts for company {CompanyId}", companyId);
                throw;
            }
        }



        #endregion

        #region Purchase Order Items

        public async Task<PurchaseOrderItem> AddPurchaseOrderItemAsync(PurchaseOrderItem item)
        {
            try
            {
                item.Id = Guid.NewGuid();
                item.CreatedDate = DateTime.UtcNow;
                item.LineTotal = (item.Quantity * item.UnitPrice) - item.DiscountAmount + item.TaxAmount;
                item.RemainingQuantity = item.Quantity;

                _context.PurchaseOrderItems.Add(item);
                await _context.SaveChangesAsync();

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding purchase order item");
                throw;
            }
        }

        public async Task<PurchaseOrderItem> UpdatePurchaseOrderItemAsync(PurchaseOrderItem item)
        {
            try
            {
                item.ModifiedDate = DateTime.UtcNow;
                item.LineTotal = (item.Quantity * item.UnitPrice) - item.DiscountAmount + item.TaxAmount;
                item.RemainingQuantity = item.Quantity - item.ReceivedQuantity;

                _context.PurchaseOrderItems.Update(item);
                await _context.SaveChangesAsync();

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating purchase order item");
                throw;
            }
        }

        public async Task<bool> RemovePurchaseOrderItemAsync(Guid itemId)
        {
            try
            {
                var item = await _context.PurchaseOrderItems.FindAsync(itemId);
                if (item == null) return false;

                _context.PurchaseOrderItems.Remove(item);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing purchase order item {ItemId}", itemId);
                throw;
            }
        }

        public async Task<IEnumerable<PurchaseOrderItem>> GetPurchaseOrderItemsAsync(Guid purchaseOrderId)
        {
            return await _context.PurchaseOrderItems
                .Include(i => i.Product)
                .Where(i => i.PurchaseOrderId == purchaseOrderId)
                .ToListAsync();
        }

        #endregion

        #region Quotation Management

        public async Task<Quotation> CreateQuotationAsync(Quotation quotation)
        {
            try
            {
                quotation.Id = Guid.NewGuid();
                quotation.CreatedDate = DateTime.UtcNow;
                quotation.Status = QuotationStatus.Draft;

                _context.Quotations.Add(quotation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Quotation {QuotationNumber} created successfully", quotation.QuotationNumber);
                return quotation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quotation {QuotationNumber}", quotation.QuotationNumber);
                throw;
            }
        }

        public async Task<Quotation?> GetQuotationByIdAsync(Guid id)
        {
            return await _context.Quotations
                .Include(q => q.Items)
                    .ThenInclude(i => i.Product)
                .Include(q => q.Company)
                .Include(q => q.Supplier)
                .Include(q => q.RequestedByUser)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Quotation?> GetQuotationByNumberAsync(string quotationNumber)
        {
            return await _context.Quotations
                .Include(q => q.Items)
                    .ThenInclude(i => i.Product)
                .Include(q => q.Company)
                .Include(q => q.Supplier)
                .FirstOrDefaultAsync(q => q.QuotationNumber == quotationNumber);
        }

        public async Task<IEnumerable<Quotation>> GetQuotationsByCompanyAsync(Guid companyId)
        {
            return await _context.Quotations
                .Include(q => q.Supplier)
                .Include(q => q.RequestedByUser)
                .Where(q => q.CompanyId == companyId)
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quotation>> GetQuotationsByStatusAsync(Guid companyId, QuotationStatus status)
        {
            return await _context.Quotations
                .Include(q => q.Supplier)
                .Include(q => q.RequestedByUser)
                .Where(q => q.CompanyId == companyId && q.Status == status)
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quotation>> GetQuotationsBySupplierAsync(Guid supplierId)
        {
            return await _context.Quotations
                .Include(q => q.Company)
                .Include(q => q.RequestedByUser)
                .Where(q => q.SupplierId == supplierId)
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quotation>> GetQuotationsAsync(Guid companyId)
        {
            return await GetQuotationsByCompanyAsync(companyId);
        }

        public async Task<Quotation> UpdateQuotationAsync(Guid id, Quotation quotation)
        {
            try
            {
                var existingQuotation = await _context.Quotations.FindAsync(id);
                if (existingQuotation == null)
                    throw new ArgumentException("Quotation not found");

                // Update properties
                existingQuotation.ModifiedDate = DateTime.UtcNow;
                // Copy other properties from quotation parameter
                
                _context.Quotations.Update(existingQuotation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Quotation {Id} updated successfully", id);
                return existingQuotation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quotation {Id}", id);
                throw;
            }
        }

        public async Task<Quotation> UpdateQuotationStatusAsync(Guid id, QuotationStatus status)
        {
            try
            {
                var quotation = await _context.Quotations.FindAsync(id);
                if (quotation == null)
                    throw new ArgumentException("Quotation not found");

                quotation.Status = status;
                quotation.ModifiedDate = DateTime.UtcNow;
                
                _context.Quotations.Update(quotation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Quotation {Id} status updated to {Status}", id, status);
                return quotation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quotation {Id} status", id);
                throw;
            }
        }

        public async Task<Quotation> UpdateQuotationAsync(Quotation quotation)
        {
            try
            {
                quotation.ModifiedDate = DateTime.UtcNow;
                _context.Quotations.Update(quotation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Quotation {QuotationNumber} updated successfully", quotation.QuotationNumber);
                return quotation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quotation {QuotationNumber}", quotation.QuotationNumber);
                throw;
            }
        }

        public async Task<bool> DeleteQuotationAsync(Guid id)
        {
            try
            {
                var quotation = await _context.Quotations.FindAsync(id);
                if (quotation == null) return false;

                _context.Quotations.Remove(quotation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Quotation {Id} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quotation {Id}", id);
                throw;
            }
        }

        public async Task<PurchaseOrder> ConvertQuotationToPurchaseOrderAsync(Guid quotationId, Guid convertedBy)
        {
            var quotation = await GetQuotationByIdAsync(quotationId);
            if (quotation == null)
                throw new ArgumentException("Quotation not found");

            var purchaseOrder = new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                OrderNumber = await GeneratePurchaseOrderNumberAsync(),
                CompanyId = quotation.CompanyId,
                SupplierId = quotation.SupplierId,
                OrderDate = DateTime.UtcNow,
                Status = PurchaseOrderStatus.Draft,
                SubTotal = quotation.SubTotal,
                TaxAmount = quotation.TaxAmount,
                DiscountAmount = quotation.DiscountAmount,
                TotalAmount = quotation.TotalAmount,
                Notes = quotation.Notes,
                PaymentTerms = quotation.PaymentTerms,
                DeliveryTerms = quotation.DeliveryTerms,
                ExpectedDeliveryDate = quotation.DeliveryDays.HasValue ? DateTime.UtcNow.AddDays(quotation.DeliveryDays.Value) : null,
                Currency = quotation.Currency,
                CreatedBy = convertedBy,
                CreatedDate = DateTime.UtcNow
            };

            _context.PurchaseOrders.Add(purchaseOrder);

            // Convert quotation items to purchase order items
            foreach (var quotationItem in quotation.Items)
            {
                var purchaseOrderItem = new PurchaseOrderItem
                {
                    Id = Guid.NewGuid(),
                    PurchaseOrderId = purchaseOrder.Id,
                    ProductId = quotationItem.ProductId,
                    Quantity = quotationItem.Quantity,
                    UnitPrice = quotationItem.UnitPrice,
                    DiscountAmount = quotationItem.DiscountAmount,
                    TaxAmount = quotationItem.TaxAmount,
                    LineTotal = quotationItem.LineTotal,
                    Description = quotationItem.Description,
                    Unit = quotationItem.Unit,
                    RemainingQuantity = quotationItem.Quantity,
                    CreatedDate = DateTime.UtcNow
                };

                _context.PurchaseOrderItems.Add(purchaseOrderItem);
            }

            // Update quotation status
            quotation.Status = QuotationStatus.ConvertedToPO;
            quotation.ConvertedToPurchaseOrderId = purchaseOrder.Id;
            quotation.ConvertedDate = DateTime.UtcNow;
            quotation.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Quotation {QuotationNumber} converted to purchase order {OrderNumber}", 
                quotation.QuotationNumber, purchaseOrder.OrderNumber);

            return purchaseOrder;
        }

        public async Task<Quotation> AcceptQuotationAsync(Guid id)
        {
            var quotation = await GetQuotationByIdAsync(id);
            if (quotation == null)
                throw new ArgumentException("Quotation not found");

            quotation.Status = QuotationStatus.Accepted;
            quotation.ModifiedDate = DateTime.UtcNow;

            return await UpdateQuotationAsync(quotation);
        }

        public async Task<Quotation> RejectQuotationAsync(Guid id, string reason)
        {
            var quotation = await GetQuotationByIdAsync(id);
            if (quotation == null)
                throw new ArgumentException("Quotation not found");

            quotation.Status = QuotationStatus.Rejected;
            quotation.Notes = string.IsNullOrEmpty(quotation.Notes) ? reason : $"{quotation.Notes}\n\nRejection Reason: {reason}";
            quotation.ModifiedDate = DateTime.UtcNow;

            return await UpdateQuotationAsync(quotation);
        }

        #endregion

        #region Quotation Items

        public async Task<QuotationItem> AddQuotationItemAsync(QuotationItem item)
        {
            try
            {
                item.Id = Guid.NewGuid();
                item.CreatedDate = DateTime.UtcNow;
                item.LineTotal = (item.Quantity * item.UnitPrice) - item.DiscountAmount + item.TaxAmount;

                _context.QuotationItems.Add(item);
                await _context.SaveChangesAsync();

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding quotation item");
                throw;
            }
        }

        public async Task<QuotationItem> UpdateQuotationItemAsync(QuotationItem item)
        {
            try
            {
                item.ModifiedDate = DateTime.UtcNow;
                item.LineTotal = (item.Quantity * item.UnitPrice) - item.DiscountAmount + item.TaxAmount;

                _context.QuotationItems.Update(item);
                await _context.SaveChangesAsync();

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quotation item");
                throw;
            }
        }

        public async Task<bool> RemoveQuotationItemAsync(Guid itemId)
        {
            try
            {
                var item = await _context.QuotationItems.FindAsync(itemId);
                if (item == null) return false;

                _context.QuotationItems.Remove(item);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing quotation item {ItemId}", itemId);
                throw;
            }
        }

        public async Task<IEnumerable<QuotationItem>> GetQuotationItemsAsync(Guid quotationId)
        {
            return await _context.QuotationItems
                .Include(i => i.Product)
                .Where(i => i.QuotationId == quotationId)
                .ToListAsync();
        }

        #endregion

        #region Goods Receipt Management

        public async Task<GoodsReceipt> CreateGoodsReceiptAsync(GoodsReceipt goodsReceipt)
        {
            try
            {
                goodsReceipt.Id = Guid.NewGuid();
                goodsReceipt.CreatedDate = DateTime.UtcNow;
                goodsReceipt.Status = GoodsReceiptStatus.Draft;

                _context.GoodsReceipts.Add(goodsReceipt);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Goods receipt {ReceiptNumber} created successfully", goodsReceipt.ReceiptNumber);
                return goodsReceipt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating goods receipt {ReceiptNumber}", goodsReceipt.ReceiptNumber);
                throw;
            }
        }

        public async Task<GoodsReceipt?> GetGoodsReceiptByIdAsync(Guid id)
        {
            return await _context.GoodsReceipts
                .Include(gr => gr.Items)
                    .ThenInclude(i => i.Product)
                .Include(gr => gr.PurchaseOrder)
                .Include(gr => gr.Company)
                .Include(gr => gr.Supplier)
                .Include(gr => gr.ReceivedByUser)
                .FirstOrDefaultAsync(gr => gr.Id == id);
        }

        public async Task<GoodsReceipt?> GetGoodsReceiptByNumberAsync(string receiptNumber)
        {
            return await _context.GoodsReceipts
                .Include(gr => gr.Items)
                    .ThenInclude(i => i.Product)
                .Include(gr => gr.PurchaseOrder)
                .Include(gr => gr.Company)
                .Include(gr => gr.Supplier)
                .FirstOrDefaultAsync(gr => gr.ReceiptNumber == receiptNumber);
        }

        public async Task<IEnumerable<GoodsReceipt>> GetGoodsReceiptsByPurchaseOrderAsync(Guid purchaseOrderId)
        {
            return await _context.GoodsReceipts
                .Include(gr => gr.ReceivedByUser)
                .Where(gr => gr.PurchaseOrderId == purchaseOrderId)
                .OrderByDescending(gr => gr.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoodsReceipt>> GetGoodsReceiptsByCompanyAsync(Guid companyId)
        {
            return await _context.GoodsReceipts
                .Include(gr => gr.PurchaseOrder)
                .Include(gr => gr.Supplier)
                .Include(gr => gr.ReceivedByUser)
                .Where(gr => gr.CompanyId == companyId)
                .OrderByDescending(gr => gr.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoodsReceipt>> GetGoodsReceiptsByStatusAsync(Guid companyId, GoodsReceiptStatus status)
        {
            return await _context.GoodsReceipts
                .Include(gr => gr.PurchaseOrder)
                .Include(gr => gr.Supplier)
                .Include(gr => gr.ReceivedByUser)
                .Where(gr => gr.CompanyId == companyId && gr.Status == status)
                .OrderByDescending(gr => gr.CreatedDate)
                .ToListAsync();
        }

        public async Task<GoodsReceipt> UpdateGoodsReceiptAsync(GoodsReceipt goodsReceipt)
        {
            try
            {
                goodsReceipt.ModifiedDate = DateTime.UtcNow;
                _context.GoodsReceipts.Update(goodsReceipt);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Goods receipt {ReceiptNumber} updated successfully", goodsReceipt.ReceiptNumber);
                return goodsReceipt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating goods receipt {ReceiptNumber}", goodsReceipt.ReceiptNumber);
                throw;
            }
        }

        public async Task<bool> DeleteGoodsReceiptAsync(Guid id)
        {
            try
            {
                var goodsReceipt = await _context.GoodsReceipts.FindAsync(id);
                if (goodsReceipt == null) return false;

                _context.GoodsReceipts.Remove(goodsReceipt);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Goods receipt {Id} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting goods receipt {Id}", id);
                throw;
            }
        }

        public async Task<GoodsReceipt> CompleteGoodsReceiptAsync(Guid id, Guid completedBy)
        {
            var goodsReceipt = await GetGoodsReceiptByIdAsync(id);
            if (goodsReceipt == null)
                throw new ArgumentException("Goods receipt not found");

            goodsReceipt.Status = GoodsReceiptStatus.Completed;
            goodsReceipt.IsCompleted = true;
            goodsReceipt.ApprovedBy = completedBy;
            goodsReceipt.ApprovedDate = DateTime.UtcNow;
            goodsReceipt.ModifiedDate = DateTime.UtcNow;

            return await UpdateGoodsReceiptAsync(goodsReceipt);
        }

        public async Task<GoodsReceipt> InspectGoodsReceiptAsync(Guid id, Guid inspectedBy)
        {
            var goodsReceipt = await GetGoodsReceiptByIdAsync(id);
            if (goodsReceipt == null)
                throw new ArgumentException("Goods receipt not found");

            goodsReceipt.Status = GoodsReceiptStatus.UnderInspection;
            goodsReceipt.InspectedBy = inspectedBy;
            goodsReceipt.InspectionDate = DateTime.UtcNow;
            goodsReceipt.ModifiedDate = DateTime.UtcNow;

            return await UpdateGoodsReceiptAsync(goodsReceipt);
        }

        #endregion

        #region Goods Receipt Items

        public async Task<GoodsReceiptItem> AddGoodsReceiptItemAsync(GoodsReceiptItem item)
        {
            try
            {
                item.Id = Guid.NewGuid();
                item.CreatedDate = DateTime.UtcNow;
                item.AcceptedQuantity = item.ReceivedQuantity - item.RejectedQuantity - item.DamagedQuantity;

                _context.GoodsReceiptItems.Add(item);
                await _context.SaveChangesAsync();

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding goods receipt item");
                throw;
            }
        }

        public async Task<GoodsReceiptItem> UpdateGoodsReceiptItemAsync(GoodsReceiptItem item)
        {
            try
            {
                item.ModifiedDate = DateTime.UtcNow;
                item.AcceptedQuantity = item.ReceivedQuantity - item.RejectedQuantity - item.DamagedQuantity;

                _context.GoodsReceiptItems.Update(item);
                await _context.SaveChangesAsync();

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating goods receipt item");
                throw;
            }
        }

        public async Task<bool> RemoveGoodsReceiptItemAsync(Guid itemId)
        {
            try
            {
                var item = await _context.GoodsReceiptItems.FindAsync(itemId);
                if (item == null) return false;

                _context.GoodsReceiptItems.Remove(item);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing goods receipt item {ItemId}", itemId);
                throw;
            }
        }

        public async Task<IEnumerable<GoodsReceiptItem>> GetGoodsReceiptItemsAsync(Guid goodsReceiptId)
        {
            return await _context.GoodsReceiptItems
                .Include(i => i.Product)
                .Include(i => i.PurchaseOrderItem)
                .Where(i => i.GoodsReceiptId == goodsReceiptId)
                .ToListAsync();
        }

        public async Task<GoodsReceiptItem> InspectGoodsReceiptItemAsync(Guid itemId, QualityStatus qualityStatus, string? notes)
        {
            var item = await _context.GoodsReceiptItems.FindAsync(itemId);
            if (item == null)
                throw new ArgumentException("Goods receipt item not found");

            item.QualityStatus = qualityStatus;
            item.QualityNotes = notes;
            item.IsInspected = true;
            item.InspectionDate = DateTime.UtcNow;
            item.ModifiedDate = DateTime.UtcNow;

            return await UpdateGoodsReceiptItemAsync(item);
        }

        #endregion

        #region Approval Management

        public async Task<Approval> CreateApprovalAsync(Approval approval)
        {
            try
            {
                approval.Id = Guid.NewGuid();
                approval.CreatedDate = DateTime.UtcNow;
                approval.Status = ApprovalStatus.Pending;

                _context.Approvals.Add(approval);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Approval created for document {DocumentType} {DocumentNumber}", 
                    approval.DocumentType, approval.DocumentNumber);
                return approval;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating approval for document {DocumentType} {DocumentNumber}", 
                    approval.DocumentType, approval.DocumentNumber);
                throw;
            }
        }

        public async Task<Approval?> GetApprovalByIdAsync(Guid id)
        {
            return await _context.Approvals
                .Include(a => a.Company)
                .Include(a => a.RequestedByUser)
                .Include(a => a.ApproverUser)
                .Include(a => a.ApprovalLevel)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Approval>> GetPendingApprovalsAsync(Guid approverId)
        {
            return await _context.Approvals
                .Include(a => a.Company)
                .Include(a => a.RequestedByUser)
                .Include(a => a.ApprovalLevel)
                .Where(a => a.ApproverId == approverId && a.Status == ApprovalStatus.Pending && a.IsActive)
                .OrderBy(a => a.RequestedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Approval>> GetApprovalsByDocumentAsync(string documentType, Guid documentId)
        {
            return await _context.Approvals
                .Include(a => a.ApproverUser)
                .Include(a => a.ApprovalLevel)
                .Where(a => a.DocumentType == documentType && a.DocumentId == documentId)
                .OrderBy(a => a.ApprovalOrder)
                .ToListAsync();
        }

        public async Task<Approval> UpdateApprovalAsync(Guid id, UpdateApprovalRequest request)
        {
            try
            {
                var approval = await GetApprovalByIdAsync(id);
                if (approval == null)
                    throw new ArgumentException("Approval not found");

                approval.Status = request.Status;
                approval.Comments = request.Comments;
                approval.RejectionReason = request.RejectionReason;
                approval.RequiredDate = request.RequiredDate;
                approval.ModifiedDate = DateTime.UtcNow;

                _context.Approvals.Update(approval);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Approval {ApprovalId} updated successfully", id);
                return approval;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating approval {ApprovalId}", id);
                throw;
            }
        }

        public async Task<Approval> ProcessApprovalAsync(Guid approvalId, ApprovalStatus status, string? comments)
        {
            var approval = await GetApprovalByIdAsync(approvalId);
            if (approval == null)
                throw new ArgumentException("Approval not found");

            approval.Status = status;
            approval.Comments = comments;
            approval.ModifiedDate = DateTime.UtcNow;

            if (status == ApprovalStatus.Approved)
            {
                approval.ApprovedDate = DateTime.UtcNow;
            }
            else if (status == ApprovalStatus.Rejected)
            {
                approval.RejectedDate = DateTime.UtcNow;
                approval.RejectionReason = comments;
            }

            _context.Approvals.Update(approval);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Approval {ApprovalId} processed with status {Status}", approvalId, status);
            return approval;
        }

        public async Task<bool> DelegateApprovalAsync(Guid approvalId, Guid delegateToUserId, string reason)
        {
            try
            {
                var approval = await GetApprovalByIdAsync(approvalId);
                if (approval == null) return false;

                approval.Status = ApprovalStatus.Delegated;
                approval.Comments = $"Delegated to user {delegateToUserId}. Reason: {reason}";
                approval.ModifiedDate = DateTime.UtcNow;

                // Create new approval for delegate
                var delegatedApproval = new Approval
                {
                    Id = Guid.NewGuid(),
                    CompanyId = approval.CompanyId,
                    DocumentType = approval.DocumentType,
                    DocumentId = approval.DocumentId,
                    DocumentNumber = approval.DocumentNumber,
                    RequestedBy = approval.RequestedBy,
                    RequestedDate = approval.RequestedDate,
                    ApprovalLevelId = approval.ApprovalLevelId,
                    ApprovalOrder = approval.ApprovalOrder,
                    ApproverId = delegateToUserId,
                    Status = ApprovalStatus.Pending,
                    DocumentAmount = approval.DocumentAmount,
                    DueDate = approval.DueDate,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Approvals.Update(approval);
                _context.Approvals.Add(delegatedApproval);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Approval {ApprovalId} delegated to user {DelegateUserId}", approvalId, delegateToUserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error delegating approval {ApprovalId}", approvalId);
                throw;
            }
        }

        #endregion

        #region Approval Level Management

        public async Task<ApprovalLevel> CreateApprovalLevelAsync(CreateApprovalLevelRequest request)
        {
            try
            {
                var approvalLevel = new ApprovalLevel
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Level = request.Level,
                    DocumentType = request.DocumentType,
                    MinAmount = request.MinAmount,
                    MaxAmount = request.MaxAmount,
                    CompanyId = new Guid(request.CompanyId.ToString().PadLeft(32, '0')), // TODO: Implement proper CompanyId mapping
                    RequiredApprovals = request.RequiredApprovals,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                _context.ApprovalLevels.Add(approvalLevel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Approval level {Name} created successfully", approvalLevel.Name);
                return approvalLevel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating approval level {Name}", request.Name);
                throw;
            }
        }

        public async Task<ApprovalLevel?> GetApprovalLevelByIdAsync(Guid id)
        {
            return await _context.ApprovalLevels
                .Include(al => al.ApprovalLevelUsers)
                    .ThenInclude(alu => alu.User)
                .FirstOrDefaultAsync(al => al.Id == id);
        }

        public async Task<IEnumerable<ApprovalLevel>> GetApprovalLevelsByCompanyAsync(Guid companyId)
        {
            return await _context.ApprovalLevels
                .Include(al => al.ApprovalLevelUsers)
                    .ThenInclude(alu => alu.User)
                .Where(al => al.CompanyId == companyId && al.IsActive)
                .OrderBy(al => al.Level)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApprovalLevel>> GetApprovalLevelsByDocumentTypeAsync(Guid companyId, string documentType)
        {
            return await _context.ApprovalLevels
                .Include(al => al.ApprovalLevelUsers)
                    .ThenInclude(alu => alu.User)
                .Where(al => al.CompanyId == companyId && al.DocumentType == documentType && al.IsActive)
                .OrderBy(al => al.Level)
                .ToListAsync();
        }

        public async Task<ApprovalLevel> UpdateApprovalLevelAsync(ApprovalLevel approvalLevel)
        {
            try
            {
                approvalLevel.ModifiedDate = DateTime.UtcNow;
                _context.ApprovalLevels.Update(approvalLevel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Approval level {Name} updated successfully", approvalLevel.Name);
                return approvalLevel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating approval level {Name}", approvalLevel.Name);
                throw;
            }
        }

        public async Task<bool> DeleteApprovalLevelAsync(Guid id)
        {
            try
            {
                var approvalLevel = await _context.ApprovalLevels.FindAsync(id);
                if (approvalLevel == null) return false;

                approvalLevel.IsActive = false;
                approvalLevel.ModifiedDate = DateTime.UtcNow;

                _context.ApprovalLevels.Update(approvalLevel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Approval level {Id} deactivated successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting approval level {Id}", id);
                throw;
            }
        }

        #endregion

        #region Reports and Analytics

        public async Task<decimal> GetTotalPurchaseAmountAsync(Guid companyId, DateTime fromDate, DateTime toDate)
        {
            return await _context.PurchaseOrders
                .Where(po => po.CompanyId == companyId && 
                           po.OrderDate >= fromDate && 
                           po.OrderDate <= toDate &&
                           po.Status != PurchaseOrderStatus.Cancelled)
                .SumAsync(po => po.TotalAmount);
        }

        public async Task<IEnumerable<object>> GetPurchaseOrdersBySupplierReportAsync(Guid companyId, DateTime fromDate, DateTime toDate)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Where(po => po.CompanyId == companyId && 
                           po.OrderDate >= fromDate && 
                           po.OrderDate <= toDate)
                .GroupBy(po => new { po.SupplierId, po.Supplier.Name })
                .Select(g => new
                {
                    SupplierId = g.Key.SupplierId,
                    SupplierName = g.Key.Name,
                    TotalOrders = g.Count(),
                    TotalAmount = g.Sum(po => po.TotalAmount),
                    AverageAmount = g.Average(po => po.TotalAmount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetPendingReceiptsReportAsync(Guid companyId)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Where(po => po.CompanyId == companyId && 
                           (po.Status == PurchaseOrderStatus.Sent || 
                            po.Status == PurchaseOrderStatus.Acknowledged ||
                            po.Status == PurchaseOrderStatus.PartiallyReceived))
                .Select(po => new
                {
                    PurchaseOrderId = po.Id,
                    OrderNumber = po.OrderNumber,
                    SupplierName = po.Supplier.Name,
                    OrderDate = po.OrderDate,
                    ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                    TotalAmount = po.TotalAmount,
                    Status = po.Status.ToString(),
                    DaysOverdue = po.ExpectedDeliveryDate.HasValue && po.ExpectedDeliveryDate < DateTime.UtcNow 
                        ? (DateTime.UtcNow - po.ExpectedDeliveryDate.Value).Days 
                        : 0
                })
                .OrderBy(x => x.ExpectedDeliveryDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetSupplierPerformanceReportAsync(Guid companyId, DateTime fromDate, DateTime toDate)
        {
            var supplierPerformance = await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.GoodsReceipts)
                .Where(po => po.CompanyId == companyId && 
                           po.OrderDate >= fromDate && 
                           po.OrderDate <= toDate &&
                           po.Status != PurchaseOrderStatus.Cancelled)
                .GroupBy(po => new { po.SupplierId, po.Supplier.Name })
                .Select(g => new
                {
                    SupplierId = g.Key.SupplierId,
                    SupplierName = g.Key.Name,
                    TotalOrders = g.Count(),
                    CompletedOrders = g.Count(po => po.Status == PurchaseOrderStatus.Completed),
                    OnTimeDeliveries = g.Count(po => po.GoodsReceipts.Any(gr => 
                        po.ExpectedDeliveryDate.HasValue && gr.ReceiptDate <= po.ExpectedDeliveryDate)),
                    TotalAmount = g.Sum(po => po.TotalAmount),
                    AverageDeliveryDays = g.Where(po => po.ExpectedDeliveryDate.HasValue && po.GoodsReceipts.Any())
                        .Average(po => po.GoodsReceipts.Min(gr => 
                            (gr.ReceiptDate - po.OrderDate).TotalDays))
                })
                .ToListAsync();

            return supplierPerformance.Select(sp => new
            {
                sp.SupplierId,
                sp.SupplierName,
                sp.TotalOrders,
                sp.CompletedOrders,
                sp.OnTimeDeliveries,
                sp.TotalAmount,
                CompletionRate = sp.TotalOrders > 0 ? (decimal)sp.CompletedOrders / sp.TotalOrders * 100 : 0,
                OnTimeDeliveryRate = sp.TotalOrders > 0 ? (decimal)sp.OnTimeDeliveries / sp.TotalOrders * 100 : 0,
                AverageDeliveryDays = sp.AverageDeliveryDays > 0 ? (decimal)sp.AverageDeliveryDays : 0m
            });
        }

        public async Task<object> GetPurchaseOrderReportAsync(PurchaseOrderReportRequest request)
        {
            try
            {
                _logger.LogInformation("Getting purchase order report for company {CompanyId}", request.CompanyId);
                
                var query = _context.PurchaseOrders
                    .Include(po => po.Supplier)
                    .Include(po => po.CreatedByUser)
                    .Where(po => po.CompanyId == request.CompanyId);

                query = query.Where(po => po.OrderDate >= request.FromDate);
                query = query.Where(po => po.OrderDate <= request.ToDate);
                    
                if (request.Status.HasValue)
                    query = query.Where(po => po.Status == request.Status.Value);
                    
                if (request.SupplierId.HasValue)
                    query = query.Where(po => po.SupplierId == request.SupplierId.Value);

                var result = await query
                    .Select(po => new
                    {
                        po.Id,
                        po.OrderNumber,
                        po.OrderDate,
                        po.TotalAmount,
                        po.Status,
                        SupplierName = po.Supplier.Name,
                        CreatedBy = po.CreatedByUser.FirstName + " " + po.CreatedByUser.LastName
                    })
                    .OrderByDescending(po => po.OrderDate)
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchase order report");
                throw;
            }
        }

        public async Task<IEnumerable<object>> GetProcurementAnalyticsAsync(Guid companyId, DateTime fromDate, DateTime toDate)
        {
            var analytics = new List<object>();

            // Monthly purchase trends
            var monthlyTrends = await _context.PurchaseOrders
                .Where(po => po.CompanyId == companyId && 
                           po.OrderDate >= fromDate && 
                           po.OrderDate <= toDate &&
                           po.Status != PurchaseOrderStatus.Cancelled)
                .GroupBy(po => new { po.OrderDate.Year, po.OrderDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalOrders = g.Count(),
                    TotalAmount = g.Sum(po => po.TotalAmount)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            analytics.Add(new { Type = "MonthlyTrends", Data = monthlyTrends });

            // Status distribution
            var statusDistribution = await _context.PurchaseOrders
                .Where(po => po.CompanyId == companyId && 
                           po.OrderDate >= fromDate && 
                           po.OrderDate <= toDate)
                .GroupBy(po => po.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count(),
                    TotalAmount = g.Sum(po => po.TotalAmount)
                })
                .ToListAsync();

            analytics.Add(new { Type = "StatusDistribution", Data = statusDistribution });

            // Top products by purchase volume
            var topProducts = await _context.PurchaseOrderItems
                .Include(poi => poi.Product)
                .Include(poi => poi.PurchaseOrder)
                .Where(poi => poi.PurchaseOrder.CompanyId == companyId && 
                            poi.PurchaseOrder.OrderDate >= fromDate && 
                            poi.PurchaseOrder.OrderDate <= toDate)
                .GroupBy(poi => new { poi.ProductId, poi.Product.Name })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalQuantity = g.Sum(poi => poi.Quantity),
                    TotalAmount = g.Sum(poi => poi.LineTotal)
                })
                .OrderByDescending(x => x.TotalAmount)
                .Take(10)
                .ToListAsync();

            analytics.Add(new { Type = "TopProducts", Data = topProducts });

            return analytics;
        }

        #endregion

        #region Additional Methods

        public async Task<object> GetQuotationReportAsync(object request)
        {
            _logger.LogInformation("Getting quotation report");
            // TODO: Implement quotation report logic
            return new { Message = "Quotation report not implemented yet" };
        }

        public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersAsync(Guid companyId)
        {
            _logger.LogInformation("Getting purchase orders for company {CompanyId}", companyId);
            return await GetPurchaseOrdersByCompanyAsync(companyId);
        }

        public async Task<IEnumerable<Approval>> GetPendingApprovalsForUserAsync(Guid userId)
        {
            return await _context.Approvals
                .Include(a => a.Company)
                .Include(a => a.RequestedByUser)
                .Include(a => a.ApprovalLevel)
                .Where(a => a.ApproverId == userId && a.Status == ApprovalStatus.Pending && a.IsActive)
                .OrderBy(a => a.RequestedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApprovalLevel>> GetApprovalLevelsAsync(Guid companyId)
        {
            return await GetApprovalLevelsByCompanyAsync(companyId);
        }

        public async Task<object> GetApprovalReportAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var approvals = await _context.Approvals
                .Include(a => a.ApprovalLevel)
                .Include(a => a.ApproverUser)
                .Include(a => a.RequestedByUser)
                .Where(a => a.CompanyId == companyId && 
                           a.RequestedDate >= startDate && 
                           a.RequestedDate <= endDate)
                .ToListAsync();

            var report = new
            {
                TotalApprovals = approvals.Count,
                PendingApprovals = approvals.Count(a => a.Status == ApprovalStatus.Pending),
                ApprovedCount = approvals.Count(a => a.Status == ApprovalStatus.Approved),
                RejectedCount = approvals.Count(a => a.Status == ApprovalStatus.Rejected),
                ApprovalsByLevel = approvals.GroupBy(a => a.ApprovalLevel.Name)
                    .Select(g => new { Level = g.Key, Count = g.Count() }),
                ApprovalsByUser = approvals.GroupBy(a => a.ApproverUser.Name)
                    .Select(g => new { User = g.Key, Count = g.Count() })
            };

            return report;
        }

        public async Task<object> GetApprovalReportAsync(object request)
        {
            _logger.LogInformation("Getting approval report with request object");
            // TODO: Implement approval report logic with request object
            return new { Message = "Approval report with request object not implemented yet" };
        }

        public async System.Threading.Tasks.Task DeleteApprovalAsync(Guid id)
        {
            try
            {
                var approval = await _context.Approvals.FindAsync(id);
                if (approval == null)
                    throw new ArgumentException("Approval not found");

                _context.Approvals.Remove(approval);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Approval {Id} deleted successfully", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting approval {Id}", id);
                throw;
            }
        }

        public async Task<ApprovalLevel> UpdateApprovalLevelAsync(Guid id, object request)
        {
            try
            {
                var approvalLevel = await _context.ApprovalLevels.FindAsync(id);
                if (approvalLevel == null)
                    throw new ArgumentException("Approval level not found");

                // TODO: Update approval level properties based on request
                approvalLevel.ModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Approval level {Id} updated successfully", id);
                return approvalLevel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating approval level {Id}", id);
                throw;
            }
        }

        public async Task<object> AddUserToApprovalLevelAsync(Guid levelId, CreateApprovalLevelUserRequest request)
        {
            try
            {
                var approvalLevelUser = new ApprovalLevelUser
                {
                    Id = Guid.NewGuid(),
                    ApprovalLevelId = levelId,
                    UserId = new Guid(request.UserId.ToString().PadLeft(32, '0')), // TODO: Implement proper UserId mapping
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                _context.ApprovalLevelUsers.Add(approvalLevelUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} added to approval level {LevelId}", request.UserId, levelId);
                return new { Id = approvalLevelUser.Id, Message = "User added successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user to approval level {LevelId}", levelId);
                throw;
            }
        }

        public async Task<object> UpdateApprovalLevelUserAsync(Guid levelId, Guid userId, UpdateApprovalLevelUserRequest request)
        {
            try
            {
                var approvalLevelUser = await _context.ApprovalLevelUsers
                    .FirstOrDefaultAsync(alu => alu.ApprovalLevelId == levelId && alu.UserId == userId);
                
                if (approvalLevelUser == null)
                    throw new ArgumentException("Approval level user not found");

                // TODO: Update approval level user properties based on request
                approvalLevelUser.ModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Approval level user updated successfully");
                return new { Message = "User updated successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating approval level user");
                throw;
            }
        }

        public async Task<bool> RemoveUserFromApprovalLevelAsync(Guid levelId, Guid userId)
        {
            try
            {
                var approvalLevelUser = await _context.ApprovalLevelUsers
                    .FirstOrDefaultAsync(alu => alu.ApprovalLevelId == levelId && alu.UserId == userId);
                
                if (approvalLevelUser == null)
                    return false;

                approvalLevelUser.IsActive = false;
                approvalLevelUser.ModifiedDate = DateTime.UtcNow;

                _context.ApprovalLevelUsers.Update(approvalLevelUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} removed from approval level {LevelId}", userId, levelId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user from approval level {LevelId}", levelId);
                throw;
            }
        }

        public async Task<object> CompareQuotationsAsync(object request)
        {
            try
            {
                _logger.LogInformation("Comparing quotations with request: {Request}", request);
                
                // TODO: Implement actual comparison logic
                throw new NotImplementedException("CompareQuotationsAsync not implemented");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing quotations");
                throw;
            }
        }

        public async Task<PurchaseOrder> UpdatePurchaseOrderAsync(Guid id, UpdatePurchaseOrderRequest request)
        {
            try
            {
                _logger.LogInformation("Updating purchase order {Id} with request: {Request}", id, request);
                
                // TODO: Implement actual update logic
                throw new NotImplementedException("UpdatePurchaseOrderAsync not implemented");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating purchase order {Id}", id);
                throw;
            }
        }

        public async Task<PurchaseOrder> UpdatePurchaseOrderStatusAsync(Guid id, UpdatePurchaseOrderStatusRequest request)
        {
            try
            {
                _logger.LogInformation("Updating purchase order status {Id} with request: {Request}", id, request);
                
                // TODO: Implement actual status update logic
                throw new NotImplementedException("UpdatePurchaseOrderStatusAsync not implemented");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating purchase order status {Id}", id);
                throw;
            }
        }

        public async Task<object> AddPurchaseOrderItemAsync(Guid purchaseOrderId, CreatePurchaseOrderItemRequest request)
        {
            try
            {
                _logger.LogInformation("Adding item to purchase order {PurchaseOrderId} with request: {Request}", purchaseOrderId, request);
                
                // TODO: Implement actual add item logic
                throw new NotImplementedException("AddPurchaseOrderItemAsync not implemented");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to purchase order {PurchaseOrderId}", purchaseOrderId);
                throw;
            }
        }

        public async Task<object> UpdatePurchaseOrderItemAsync(Guid purchaseOrderId, Guid itemId, UpdatePurchaseOrderItemRequest request)
        {
            try
            {
                _logger.LogInformation("Updating item {ItemId} in purchase order {PurchaseOrderId} with request: {Request}", itemId, purchaseOrderId, request);
                
                // TODO: Implement actual update item logic
                throw new NotImplementedException("UpdatePurchaseOrderItemAsync not implemented");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item {ItemId} in purchase order {PurchaseOrderId}", itemId, purchaseOrderId);
                throw;
            }
        }

        public async System.Threading.Tasks.Task RemovePurchaseOrderItemAsync(Guid purchaseOrderId, Guid itemId)
        {
            try
            {
                _logger.LogInformation("Removing item {ItemId} from purchase order {PurchaseOrderId}", itemId, purchaseOrderId);
                
                // TODO: Implement actual remove item logic
                throw new NotImplementedException("RemovePurchaseOrderItemAsync not implemented");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item {ItemId} from purchase order {PurchaseOrderId}", itemId, purchaseOrderId);
                throw;
            }
        }

        #endregion

        #region Private Helper Methods

        private async Task<string> GeneratePurchaseOrderNumberAsync()
        {
            var lastOrder = await _context.PurchaseOrders
                .OrderByDescending(po => po.CreatedDate)
                .FirstOrDefaultAsync();

            var lastNumber = 0;
            if (lastOrder != null && lastOrder.OrderNumber.StartsWith("PO"))
            {
                var numberPart = lastOrder.OrderNumber.Substring(2);
                if (int.TryParse(numberPart, out var parsed))
                {
                    lastNumber = parsed;
                }
            }

            return $"PO{(lastNumber + 1):D6}";
        }

        public async Task<IEnumerable<ApprovalResponse>> GetApprovalsAsync(ApprovalFilterRequest filter)
        {
            var query = _context.Approvals.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.DocumentType))
                query = query.Where(a => a.DocumentType == filter.DocumentType);

            if (filter.Status.HasValue)
                query = query.Where(a => a.Status == filter.Status.Value);

            if (filter.UserId.HasValue)
            {
                // TODO: Implement proper UserId to Guid mapping
                var userGuid = new Guid(filter.UserId.Value.ToString().PadLeft(32, '0'));
                query = query.Where(a => a.RequestedBy == userGuid);
            }

            if (filter.StartDate.HasValue)
                query = query.Where(a => a.RequestedDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(a => a.RequestedDate <= filter.EndDate.Value);

            if (filter.MinAmount.HasValue)
                query = query.Where(a => a.DocumentAmount >= filter.MinAmount.Value);

            if (filter.MaxAmount.HasValue)
                query = query.Where(a => a.DocumentAmount <= filter.MaxAmount.Value);

            if (!string.IsNullOrEmpty(filter.SearchTerm))
                query = query.Where(a => a.DocumentNumber.Contains(filter.SearchTerm) || 
                                        a.Comments.Contains(filter.SearchTerm));

            // Include related entities
            query = query.Include(a => a.Company)
                         .Include(a => a.RequestedByUser)
                         .Include(a => a.ApprovalLevel);

            // Apply pagination
            var totalCount = await query.CountAsync();
            var approvals = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .OrderByDescending(a => a.RequestedDate)
                .ToListAsync();

            // Map to response DTOs
            return approvals.Select(a => new ApprovalResponse
            {
                Id = (int)a.Id.GetHashCode(), // TODO: Proper ID mapping
                CompanyId = (int)a.CompanyId.GetHashCode(), // TODO: Proper CompanyId mapping
                CompanyName = a.Company?.Name ?? string.Empty,
                DocumentType = a.DocumentType,
                DocumentId = (int)a.DocumentId.GetHashCode(), // TODO: Proper DocumentId mapping
                DocumentNumber = a.DocumentNumber,
                ApprovalLevelId = (int)a.ApprovalLevelId.GetHashCode(), // TODO: Proper ApprovalLevelId mapping
                ApprovalLevelName = a.ApprovalLevel?.Name ?? string.Empty,
                Level = a.ApprovalLevel?.Level ?? 0,
                UserId = (int)a.RequestedBy.GetHashCode(), // TODO: Proper UserId mapping
                UserName = a.RequestedByUser?.Name ?? string.Empty,
                Status = a.Status,
                DocumentAmount = a.DocumentAmount,
                Comments = a.Comments,
                RejectionReason = a.RejectionReason,
                DueDate = a.DueDate,
                ApprovedAt = a.ApprovedDate,
                CreatedAt = a.CreatedDate,
                UpdatedAt = a.ModifiedDate ?? DateTime.UtcNow
            });
        }

        #endregion
    }
}