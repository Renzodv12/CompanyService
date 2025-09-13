using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.Procurement
{
    public class CreateQuotationRequest
    {
        public int CompanyId { get; set; }
        public int SupplierId { get; set; }
        public string QuotationNumber { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime? ValidUntil { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public string? PaymentTerms { get; set; }
        public string? DeliveryTerms { get; set; }
        public List<CreateQuotationItemRequest> Items { get; set; } = new();
    }

    public class CreateQuotationItemRequest
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal LineTotal { get; set; }
        public string? Notes { get; set; }
        public string? Specifications { get; set; }
        public int? LeadTimeDays { get; set; }
    }

    public class UpdateQuotationRequest
    {
        public int SupplierId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ValidUntil { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public string? PaymentTerms { get; set; }
        public string? DeliveryTerms { get; set; }
        public List<UpdateQuotationItemRequest> Items { get; set; } = new();
    }

    public class UpdateQuotationItemRequest
    {
        public Guid? Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal LineTotal { get; set; }
        public string? Notes { get; set; }
        public string? Specifications { get; set; }
        public int? LeadTimeDays { get; set; }
    }

    public class QuotationResponse
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string QuotationNumber { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime? ResponseDate { get; set; }
        public DateTime? ValidUntil { get; set; }
        public QuotationStatus Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public string? PaymentTerms { get; set; }
        public string? DeliveryTerms { get; set; }
        public int? RequestedByUserId { get; set; }
        public string? RequestedByUserName { get; set; }
        public int? ReviewedByUserId { get; set; }
        public string? ReviewedByUserName { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public int? PurchaseOrderId { get; set; }
        public string? PurchaseOrderNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<QuotationItemResponse> Items { get; set; } = new();
    }

    public class QuotationItemResponse
    {
        public int Id { get; set; }
        public int QuotationId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal LineTotal { get; set; }
        public string? Notes { get; set; }
        public string? Specifications { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class QuotationSummaryResponse
    {
        public int Id { get; set; }
        public string QuotationNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime? ValidUntil { get; set; }
        public QuotationStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public bool IsExpired { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateQuotationStatusRequest
    {
        public QuotationStatus Status { get; set; }
        public string? Notes { get; set; }
    }

    public class ConvertQuotationToPurchaseOrderRequest
    {
        public Guid QuotationId { get; set; }
        public string? Notes { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string? PaymentTerms { get; set; }
        public string? DeliveryTerms { get; set; }
        public List<Guid>? SelectedItemIds { get; set; }
        public List<Guid>? SelectedItems { get; set; }
        public string? PurchaseOrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
    }

    public class QuotationFilterRequest
    {
        public Guid CompanyId { get; set; }
        public Guid? SupplierId { get; set; }
        public QuotationStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool? IncludeExpired { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class QuotationComparisonRequest
    {
        public List<int> QuotationIds { get; set; } = new();
    }

    public class QuotationComparisonResponse
    {
        public List<QuotationComparisonItem> Quotations { get; set; } = new();
        public List<ProductComparisonItem> ProductComparisons { get; set; } = new();
    }

    public class QuotationComparisonItem
    {
        public int Id { get; set; }
        public string QuotationNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string? PaymentTerms { get; set; }
        public string? DeliveryTerms { get; set; }
    }

    public class ProductComparisonItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public List<ProductQuotationDetail> QuotationDetails { get; set; } = new();
    }

    public class ProductQuotationDetail
    {
        public int QuotationId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public string? Specifications { get; set; }
    }

    public class QuotationReportRequest
    {
        public int CompanyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? SupplierId { get; set; }
        public QuotationStatus? Status { get; set; }
        public string? ReportType { get; set; }
    }

    public class QuotationReportResponse
    {
        public string ReportType { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; }
        public int TotalQuotations { get; set; }
        public decimal TotalAmount { get; set; }
        public List<QuotationSummary> QuotationSummaries { get; set; } = new();
        public List<SupplierQuotationSummary> SupplierSummaries { get; set; } = new();
    }

    public class QuotationSummary
    {
        public int Id { get; set; }
        public string QuotationNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public QuotationStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class SupplierQuotationSummary
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int QuotationCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AverageAmount { get; set; }
    }
}