using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Interfaces
{
    public interface IProcurementService
    {
        // Purchase Order Management
        Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(Guid id);
        Task<PurchaseOrder?> GetPurchaseOrderByNumberAsync(string orderNumber);
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByCompanyAsync(Guid companyId);
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(Guid companyId, PurchaseOrderStatus status);
        Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<bool> DeletePurchaseOrderAsync(Guid id);
        Task<PurchaseOrder> ApprovePurchaseOrderAsync(Guid id, Guid approvedBy);
        Task<PurchaseOrder> RejectPurchaseOrderAsync(Guid id, Guid rejectedBy, string reason);
        Task<PurchaseOrder> SendPurchaseOrderAsync(Guid id);
        Task<PurchaseOrder> CancelPurchaseOrderAsync(Guid id, string reason);
        
        // Purchase Order Items
        Task<PurchaseOrderItem> AddPurchaseOrderItemAsync(PurchaseOrderItem item);
        Task<PurchaseOrderItem> UpdatePurchaseOrderItemAsync(PurchaseOrderItem item);
        Task<bool> RemovePurchaseOrderItemAsync(Guid itemId);
        Task<IEnumerable<PurchaseOrderItem>> GetPurchaseOrderItemsAsync(Guid purchaseOrderId);
        
        // Quotation Management
        Task<Quotation> CreateQuotationAsync(Quotation quotation);
        Task<Quotation?> GetQuotationByIdAsync(Guid id);
        Task<Quotation?> GetQuotationByNumberAsync(string quotationNumber);
        Task<IEnumerable<Quotation>> GetQuotationsByCompanyAsync(Guid companyId);
        Task<IEnumerable<Quotation>> GetQuotationsAsync(Guid companyId);
        Task<IEnumerable<Quotation>> GetQuotationsByStatusAsync(Guid companyId, QuotationStatus status);
        Task<IEnumerable<Quotation>> GetQuotationsBySupplierAsync(Guid supplierId);
        Task<Quotation> UpdateQuotationAsync(Guid id, Quotation quotation);
        Task<Quotation> UpdateQuotationStatusAsync(Guid id, QuotationStatus status);
        Task<bool> DeleteQuotationAsync(Guid id);
        Task<PurchaseOrder> ConvertQuotationToPurchaseOrderAsync(Guid quotationId, Guid convertedBy);
        Task<Quotation> AcceptQuotationAsync(Guid id);
        Task<Quotation> RejectQuotationAsync(Guid id, string reason);
        
        // Quotation Items
        Task<QuotationItem> AddQuotationItemAsync(QuotationItem item);
        Task<QuotationItem> UpdateQuotationItemAsync(QuotationItem item);
        Task<bool> RemoveQuotationItemAsync(Guid itemId);
        Task<IEnumerable<QuotationItem>> GetQuotationItemsAsync(Guid quotationId);
        
        // Goods Receipt Management
        Task<GoodsReceipt> CreateGoodsReceiptAsync(GoodsReceipt goodsReceipt);
        Task<GoodsReceipt?> GetGoodsReceiptByIdAsync(Guid id);
        Task<GoodsReceipt?> GetGoodsReceiptByNumberAsync(string receiptNumber);
        Task<IEnumerable<GoodsReceipt>> GetGoodsReceiptsByPurchaseOrderAsync(Guid purchaseOrderId);
        Task<IEnumerable<GoodsReceipt>> GetGoodsReceiptsByCompanyAsync(Guid companyId);
        Task<IEnumerable<GoodsReceipt>> GetGoodsReceiptsByStatusAsync(Guid companyId, GoodsReceiptStatus status);
        Task<GoodsReceipt> UpdateGoodsReceiptAsync(GoodsReceipt goodsReceipt);
        Task<bool> DeleteGoodsReceiptAsync(Guid id);
        Task<GoodsReceipt> CompleteGoodsReceiptAsync(Guid id, Guid completedBy);
        Task<GoodsReceipt> InspectGoodsReceiptAsync(Guid id, Guid inspectedBy);
        
        // Goods Receipt Items
        Task<GoodsReceiptItem> AddGoodsReceiptItemAsync(GoodsReceiptItem item);
        Task<GoodsReceiptItem> UpdateGoodsReceiptItemAsync(GoodsReceiptItem item);
        Task<bool> RemoveGoodsReceiptItemAsync(Guid itemId);
        Task<IEnumerable<GoodsReceiptItem>> GetGoodsReceiptItemsAsync(Guid goodsReceiptId);
        Task<GoodsReceiptItem> InspectGoodsReceiptItemAsync(Guid itemId, QualityStatus qualityStatus, string? notes);
        
        // Approval Management
        Task<Approval> CreateApprovalAsync(Approval approval);
        Task<Approval?> GetApprovalByIdAsync(Guid id);
        Task<IEnumerable<Approval>> GetPendingApprovalsAsync(Guid approverId);
        Task<IEnumerable<Approval>> GetPendingApprovalsForUserAsync(Guid userId);
        Task<IEnumerable<Approval>> GetApprovalsByDocumentAsync(string documentType, Guid documentId);
        Task<Approval> UpdateApprovalAsync(Guid id, UpdateApprovalRequest request);
        Task<Approval> ProcessApprovalAsync(Guid approvalId, ApprovalStatus status, string? comments);
        Task<bool> DelegateApprovalAsync(Guid approvalId, Guid delegateToUserId, string reason);
        Task<IEnumerable<ApprovalResponse>> GetApprovalsAsync(ApprovalFilterRequest filter);
        
        // Approval Level Management
        Task<ApprovalLevel> CreateApprovalLevelAsync(CreateApprovalLevelRequest request);
        Task<ApprovalLevel?> GetApprovalLevelByIdAsync(Guid id);
        Task<IEnumerable<ApprovalLevel>> GetApprovalLevelsByCompanyAsync(Guid companyId);
        Task<IEnumerable<ApprovalLevel>> GetApprovalLevelsAsync(Guid companyId);
        Task<IEnumerable<ApprovalLevel>> GetApprovalLevelsByDocumentTypeAsync(Guid companyId, string documentType);
        Task<ApprovalLevel> UpdateApprovalLevelAsync(ApprovalLevel approvalLevel);
        Task<bool> DeleteApprovalLevelAsync(Guid id);
        Task<object> AddUserToApprovalLevelAsync(Guid levelId, CreateApprovalLevelUserRequest request);
        Task<object> UpdateApprovalLevelUserAsync(Guid levelId, Guid userId, UpdateApprovalLevelUserRequest request);
        Task<bool> RemoveUserFromApprovalLevelAsync(Guid levelId, Guid userId);
        
        // Reports and Analytics
        Task<decimal> GetTotalPurchaseAmountAsync(Guid companyId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<object>> GetPurchaseOrdersBySupplierReportAsync(Guid companyId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<object>> GetPendingReceiptsReportAsync(Guid companyId);
        Task<IEnumerable<object>> GetSupplierPerformanceReportAsync(Guid companyId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<object>> GetProcurementAnalyticsAsync(Guid companyId, DateTime fromDate, DateTime toDate);
        Task<object> GetQuotationReportAsync(object request);
        Task<object> GetApprovalReportAsync(Guid companyId, DateTime fromDate, DateTime toDate);
        Task<object> GetPurchaseOrderReportAsync(PurchaseOrderReportRequest request);
        System.Threading.Tasks.Task DeleteApprovalAsync(Guid id);
        System.Threading.Tasks.Task<ApprovalLevel> UpdateApprovalLevelAsync(Guid id, object request);
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersAsync(Guid companyId);
        Task<object> CompareQuotationsAsync(object request);
        Task<PurchaseOrder> UpdatePurchaseOrderAsync(Guid id, UpdatePurchaseOrderRequest request);
        Task<PurchaseOrder> UpdatePurchaseOrderStatusAsync(Guid id, UpdatePurchaseOrderStatusRequest request);
        Task<object> AddPurchaseOrderItemAsync(Guid purchaseOrderId, CreatePurchaseOrderItemRequest request);
        Task<object> UpdatePurchaseOrderItemAsync(Guid purchaseOrderId, Guid itemId, UpdatePurchaseOrderItemRequest request);
        System.Threading.Tasks.Task RemovePurchaseOrderItemAsync(Guid purchaseOrderId, Guid itemId);
        
        // Goods Receipt methods
          Task<IEnumerable<GoodsReceipt>> GetGoodsReceiptsAsync(Guid companyId);
    }
}