using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.Procurement
{
    public class CreatePurchaseOrderRequest
    {
        public Guid CompanyId { get; set; }
        public Guid SupplierId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public string? PaymentTerms { get; set; }
        public string? DeliveryTerms { get; set; }
        public List<CreatePurchaseOrderItemRequest> Items { get; set; } = new();
    }

    public class CreatePurchaseOrderItemRequest
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal LineTotal { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdatePurchaseOrderRequest
    {
        public Guid SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public string? PaymentTerms { get; set; }
        public string? DeliveryTerms { get; set; }
        public List<UpdatePurchaseOrderItemRequest> Items { get; set; } = new();
    }

    public class UpdatePurchaseOrderItemRequest
    {
        public Guid? Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal LineTotal { get; set; }
        public string? Notes { get; set; }
    }

    public class PurchaseOrderResponse
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public string? PaymentTerms { get; set; }
        public string? DeliveryTerms { get; set; }
        public int? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public int? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<PurchaseOrderItemResponse> Items { get; set; } = new();
    }

    public class PurchaseOrderItemResponse
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal LineTotal { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PurchaseOrderSummaryResponse
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdatePurchaseOrderStatusRequest
    {
        public PurchaseOrderStatus Status { get; set; }
        public string? Notes { get; set; }
    }

    public class PurchaseOrderFilterRequest
    {
        public Guid CompanyId { get; set; }
        public Guid? SupplierId { get; set; }
        public PurchaseOrderStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? OrderNumber { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PurchaseOrderReportRequest
    {
        public Guid CompanyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public Guid? SupplierId { get; set; }
        public PurchaseOrderStatus? Status { get; set; }
        public string? ReportType { get; set; }
    }

    public class PurchaseOrderReportResponse
    {
        public string ReportType { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PurchaseOrderSummary> OrderSummaries { get; set; } = new();
        public List<SupplierSummary> SupplierSummaries { get; set; } = new();
    }

    public class PurchaseOrderSummary
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class SupplierSummary
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}