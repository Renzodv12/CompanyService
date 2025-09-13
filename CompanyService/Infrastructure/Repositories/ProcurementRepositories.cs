using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using CompanyService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.Repositories
{
    public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PurchaseOrder>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _context.PurchaseOrders
                .Where(po => po.CompanyId == companyId)
                .Include(po => po.Supplier)
                .Include(po => po.Items)
                    .ThenInclude(item => item.Product)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(Guid companyId, PurchaseOrderStatus status)
        {
            return await _context.PurchaseOrders
                .Where(po => po.CompanyId == companyId && po.Status == status)
                .Include(po => po.Supplier)
                .Include(po => po.Items)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> GetBySupplierIdAsync(Guid supplierId)
        {
            return await _context.PurchaseOrders
                .Where(po => po.SupplierId == supplierId)
                .Include(po => po.Company)
                .Include(po => po.Items)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();
        }

        public async Task<PurchaseOrder?> GetByNumberAsync(string orderNumber)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(po => po.OrderNumber == orderNumber);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetPendingApprovalsAsync(Guid companyId)
        {
            return await _context.PurchaseOrders
                .Where(po => po.CompanyId == companyId && po.Status == PurchaseOrderStatus.PendingApproval)
                .Include(po => po.Supplier)
                .Include(po => po.Items)
                .OrderBy(po => po.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> GetByDateRangeAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            return await _context.PurchaseOrders
                .Where(po => po.CompanyId == companyId && 
                           po.OrderDate >= startDate && 
                           po.OrderDate <= endDate)
                .Include(po => po.Supplier)
                .Include(po => po.Items)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }
    }

    public class PurchaseOrderItemRepository : Repository<PurchaseOrderItem>, IPurchaseOrderItemRepository
    {
        public PurchaseOrderItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PurchaseOrderItem>> GetByPurchaseOrderIdAsync(Guid purchaseOrderId)
        {
            return await _context.PurchaseOrderItems
                .Where(item => item.PurchaseOrderId == purchaseOrderId)
                .Include(item => item.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrderItem>> GetByProductIdAsync(Guid productId)
        {
            return await _context.PurchaseOrderItems
                .Where(item => item.ProductId == productId)
                .Include(item => item.PurchaseOrder)
                    .ThenInclude(po => po.Supplier)
                .ToListAsync();
        }
    }

    public class QuotationRepository : Repository<Quotation>, IQuotationRepository
    {
        public QuotationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Quotation>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _context.Quotations
                .Where(q => q.CompanyId == companyId)
                .Include(q => q.Supplier)
                .Include(q => q.Items)
                    .ThenInclude(item => item.Product)
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quotation>> GetByStatusAsync(Guid companyId, QuotationStatus status)
        {
            return await _context.Quotations
                .Where(q => q.CompanyId == companyId && q.Status == status)
                .Include(q => q.Supplier)
                .Include(q => q.Items)
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quotation>> GetBySupplierIdAsync(Guid supplierId)
        {
            return await _context.Quotations
                .Where(q => q.SupplierId == supplierId)
                .Include(q => q.Company)
                .Include(q => q.Items)
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<Quotation?> GetByNumberAsync(string quotationNumber)
        {
            return await _context.Quotations
                .Include(q => q.Supplier)
                .Include(q => q.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(q => q.QuotationNumber == quotationNumber);
        }

        public async Task<IEnumerable<Quotation>> GetExpiringQuotationsAsync(Guid companyId, DateTime beforeDate)
        {
            return await _context.Quotations
                .Where(q => q.CompanyId == companyId && 
                           q.ValidUntil.HasValue && 
                           q.ValidUntil.Value <= beforeDate &&
                           q.Status == QuotationStatus.Received)
                .Include(q => q.Supplier)
                .Include(q => q.Items)
                .OrderBy(q => q.ValidUntil)
                .ToListAsync();
        }
    }

    public class QuotationItemRepository : Repository<QuotationItem>, IQuotationItemRepository
    {
        public QuotationItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<QuotationItem>> GetByQuotationIdAsync(Guid quotationId)
        {
            return await _context.QuotationItems
                .Where(item => item.QuotationId == quotationId)
                .Include(item => item.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuotationItem>> GetByProductIdAsync(Guid productId)
        {
            return await _context.QuotationItems
                .Where(item => item.ProductId == productId)
                .Include(item => item.Quotation)
                    .ThenInclude(q => q.Supplier)
                .ToListAsync();
        }
    }

    public class ApprovalRepository : Repository<Approval>, IApprovalRepository
    {
        public ApprovalRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Approval>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _context.Approvals
                .Where(a => a.CompanyId == companyId)
                .Include(a => a.RequestedByUser)
                .Include(a => a.ApprovalLevel)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Approval>> GetByStatusAsync(Guid companyId, ApprovalStatus status)
        {
            return await _context.Approvals
                .Where(a => a.CompanyId == companyId && a.Status == status)
                .Include(a => a.RequestedByUser)
                .Include(a => a.ApprovalLevel)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Approval>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Approvals
                .Where(a => a.RequestedBy == userId)
                .Include(a => a.Company)
                .Include(a => a.ApprovalLevel)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Approval>> GetPendingApprovalsAsync(Guid companyId)
        {
            return await _context.Approvals
                .Where(a => a.CompanyId == companyId && a.Status == ApprovalStatus.Pending)
                .Include(a => a.RequestedByUser)
                .Include(a => a.ApprovalLevel)
                .OrderBy(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Approval>> GetByDocumentAsync(string documentType, Guid documentId)
        {
            return await _context.Approvals
                .Where(a => a.DocumentType == documentType && a.DocumentId == documentId)
                .Include(a => a.RequestedByUser)
                .Include(a => a.ApprovalLevel)
                .OrderBy(a => a.CreatedDate)
                .ToListAsync();
        }
    }

    public class ApprovalLevelRepository : Repository<ApprovalLevel>, IApprovalLevelRepository
    {
        public ApprovalLevelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ApprovalLevel>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _context.ApprovalLevels
                .Where(al => al.CompanyId == companyId)
                .Include(al => al.ApprovalLevelUsers)
                    .ThenInclude(alu => alu.User)
                .OrderBy(al => al.Level)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApprovalLevel>> GetByDocumentTypeAsync(Guid companyId, string documentType)
        {
            return await _context.ApprovalLevels
                .Where(al => al.CompanyId == companyId && al.DocumentType == documentType)
                .Include(al => al.ApprovalLevelUsers)
                    .ThenInclude(alu => alu.User)
                .OrderBy(al => al.Level)
                .ToListAsync();
        }

        public async Task<ApprovalLevel?> GetByLevelAsync(Guid companyId, string documentType, int level)
        {
            return await _context.ApprovalLevels
                .Include(al => al.ApprovalLevelUsers)
                    .ThenInclude(alu => alu.User)
                .FirstOrDefaultAsync(al => al.CompanyId == companyId && 
                                          al.DocumentType == documentType && 
                                          al.Level == level);
        }
    }

    public class ApprovalLevelUserRepository : Repository<ApprovalLevelUser>, IApprovalLevelUserRepository
    {
        public ApprovalLevelUserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ApprovalLevelUser>> GetByApprovalLevelIdAsync(Guid approvalLevelId)
        {
            return await _context.ApprovalLevelUsers
                .Where(alu => alu.ApprovalLevelId == approvalLevelId)
                .Include(alu => alu.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApprovalLevelUser>> GetByUserIdAsync(Guid userId)
        {
            return await _context.ApprovalLevelUsers
                .Where(alu => alu.UserId == userId)
                .Include(alu => alu.ApprovalLevel)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApprovalLevelUser>> GetActiveByUserIdAsync(Guid userId)
        {
            return await _context.ApprovalLevelUsers
                .Where(alu => alu.UserId == userId && alu.IsActive)
                .Include(alu => alu.ApprovalLevel)
                .ToListAsync();
        }
    }

    public class GoodsReceiptRepository : Repository<GoodsReceipt>, IGoodsReceiptRepository
    {
        public GoodsReceiptRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<GoodsReceipt>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _context.GoodsReceipts
                .Where(gr => gr.CompanyId == companyId)
                .Include(gr => gr.PurchaseOrder)
                .Include(gr => gr.Supplier)
                .Include(gr => gr.Items)
                    .ThenInclude(item => item.Product)
                .OrderByDescending(gr => gr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoodsReceipt>> GetByStatusAsync(Guid companyId, GoodsReceiptStatus status)
        {
            return await _context.GoodsReceipts
                .Where(gr => gr.CompanyId == companyId && gr.Status == status)
                .Include(gr => gr.PurchaseOrder)
                .Include(gr => gr.Supplier)
                .Include(gr => gr.Items)
                .OrderByDescending(gr => gr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoodsReceipt>> GetByPurchaseOrderIdAsync(Guid purchaseOrderId)
        {
            return await _context.GoodsReceipts
                .Where(gr => gr.PurchaseOrderId == purchaseOrderId)
                .Include(gr => gr.Supplier)
                .Include(gr => gr.Items)
                .OrderByDescending(gr => gr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoodsReceipt>> GetBySupplierIdAsync(Guid supplierId)
        {
            return await _context.GoodsReceipts
                .Where(gr => gr.SupplierId == supplierId)
                .Include(gr => gr.Company)
                .Include(gr => gr.PurchaseOrder)
                .Include(gr => gr.Items)
                .OrderByDescending(gr => gr.CreatedAt)
                .ToListAsync();
        }

        public async Task<GoodsReceipt?> GetByReceiptNumberAsync(string receiptNumber)
        {
            return await _context.GoodsReceipts
                .Include(gr => gr.PurchaseOrder)
                .Include(gr => gr.Supplier)
                .Include(gr => gr.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(gr => gr.ReceiptNumber == receiptNumber);
        }

        public async Task<IEnumerable<GoodsReceipt>> GetByDateRangeAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            return await _context.GoodsReceipts
                .Where(gr => gr.CompanyId == companyId && 
                           gr.ReceiptDate >= startDate && 
                           gr.ReceiptDate <= endDate)
                .Include(gr => gr.PurchaseOrder)
                .Include(gr => gr.Supplier)
                .Include(gr => gr.Items)
                .OrderByDescending(gr => gr.ReceiptDate)
                .ToListAsync();
        }
    }

    public class GoodsReceiptItemRepository : Repository<GoodsReceiptItem>, IGoodsReceiptItemRepository
    {
        public GoodsReceiptItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<GoodsReceiptItem>> GetByGoodsReceiptIdAsync(Guid goodsReceiptId)
        {
            return await _context.GoodsReceiptItems
                .Where(item => item.GoodsReceiptId == goodsReceiptId)
                .Include(item => item.Product)
                .Include(item => item.PurchaseOrderItem)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoodsReceiptItem>> GetByPurchaseOrderItemIdAsync(Guid purchaseOrderItemId)
        {
            return await _context.GoodsReceiptItems
                .Where(item => item.PurchaseOrderItemId == purchaseOrderItemId)
                .Include(item => item.GoodsReceipt)
                .Include(item => item.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoodsReceiptItem>> GetByProductIdAsync(Guid productId)
        {
            return await _context.GoodsReceiptItems
                .Where(item => item.ProductId == productId)
                .Include(item => item.GoodsReceipt)
                    .ThenInclude(gr => gr.Supplier)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoodsReceiptItem>> GetByQualityStatusAsync(Guid companyId, QualityStatus qualityStatus)
        {
            return await _context.GoodsReceiptItems
                .Where(item => item.GoodsReceipt.CompanyId == companyId && item.QualityStatus == qualityStatus)
                .Include(item => item.GoodsReceipt)
                .Include(item => item.Product)
                .ToListAsync();
        }
    }
}