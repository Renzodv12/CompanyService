using CompanyService.Core.Entities;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Interfaces
{
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        Task<IEnumerable<PurchaseOrder>> GetByCompanyIdAsync(Guid companyId);
        Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(Guid companyId, PurchaseOrderStatus status);
        Task<IEnumerable<PurchaseOrder>> GetBySupplierIdAsync(Guid supplierId);
        Task<PurchaseOrder?> GetByNumberAsync(string orderNumber);
        Task<IEnumerable<PurchaseOrder>> GetPendingApprovalsAsync(Guid companyId);
        Task<IEnumerable<PurchaseOrder>> GetByDateRangeAsync(Guid companyId, DateTime startDate, DateTime endDate);
    }

    public interface IPurchaseOrderItemRepository : IRepository<PurchaseOrderItem>
    {
        Task<IEnumerable<PurchaseOrderItem>> GetByPurchaseOrderIdAsync(Guid purchaseOrderId);
        Task<IEnumerable<PurchaseOrderItem>> GetByProductIdAsync(Guid productId);
    }

    public interface IQuotationRepository : IRepository<Quotation>
    {
        Task<IEnumerable<Quotation>> GetByCompanyIdAsync(Guid companyId);
        Task<IEnumerable<Quotation>> GetByStatusAsync(Guid companyId, QuotationStatus status);
        Task<IEnumerable<Quotation>> GetBySupplierIdAsync(Guid supplierId);
        Task<Quotation?> GetByNumberAsync(string quotationNumber);
        Task<IEnumerable<Quotation>> GetExpiringQuotationsAsync(Guid companyId, DateTime beforeDate);
    }

    public interface IQuotationItemRepository : IRepository<QuotationItem>
    {
        Task<IEnumerable<QuotationItem>> GetByQuotationIdAsync(Guid quotationId);
        Task<IEnumerable<QuotationItem>> GetByProductIdAsync(Guid productId);
    }

    public interface IApprovalRepository : IRepository<Approval>
    {
        Task<IEnumerable<Approval>> GetByCompanyIdAsync(Guid companyId);
        Task<IEnumerable<Approval>> GetByStatusAsync(Guid companyId, ApprovalStatus status);
        Task<IEnumerable<Approval>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Approval>> GetPendingApprovalsAsync(Guid companyId);
        Task<IEnumerable<Approval>> GetByDocumentAsync(string documentType, Guid documentId);
    }

    public interface IApprovalLevelRepository : IRepository<ApprovalLevel>
    {
        Task<IEnumerable<ApprovalLevel>> GetByCompanyIdAsync(Guid companyId);
        Task<IEnumerable<ApprovalLevel>> GetByDocumentTypeAsync(Guid companyId, string documentType);
        Task<ApprovalLevel?> GetByLevelAsync(Guid companyId, string documentType, int level);
    }

    public interface IApprovalLevelUserRepository : IRepository<ApprovalLevelUser>
    {
        Task<IEnumerable<ApprovalLevelUser>> GetByApprovalLevelIdAsync(Guid approvalLevelId);
        Task<IEnumerable<ApprovalLevelUser>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<ApprovalLevelUser>> GetActiveByUserIdAsync(Guid userId);
    }

    public interface IGoodsReceiptRepository : IRepository<GoodsReceipt>
    {
        Task<IEnumerable<GoodsReceipt>> GetByCompanyIdAsync(Guid companyId);
        Task<IEnumerable<GoodsReceipt>> GetByStatusAsync(Guid companyId, GoodsReceiptStatus status);
        Task<IEnumerable<GoodsReceipt>> GetByPurchaseOrderIdAsync(Guid purchaseOrderId);
        Task<IEnumerable<GoodsReceipt>> GetBySupplierIdAsync(Guid supplierId);
        Task<GoodsReceipt?> GetByReceiptNumberAsync(string receiptNumber);
        Task<IEnumerable<GoodsReceipt>> GetByDateRangeAsync(Guid companyId, DateTime startDate, DateTime endDate);
    }

    public interface IGoodsReceiptItemRepository : IRepository<GoodsReceiptItem>
    {
        Task<IEnumerable<GoodsReceiptItem>> GetByGoodsReceiptIdAsync(Guid goodsReceiptId);
        Task<IEnumerable<GoodsReceiptItem>> GetByPurchaseOrderItemIdAsync(Guid purchaseOrderItemId);
        Task<IEnumerable<GoodsReceiptItem>> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<GoodsReceiptItem>> GetByQualityStatusAsync(Guid companyId, QualityStatus qualityStatus);
    }
}