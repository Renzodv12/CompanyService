using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.Procurement
{
    public class CreateGoodsReceiptRequest
    {
        public Guid CompanyId { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public Guid SupplierId { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public string? DeliveryNote { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? TransportCompany { get; set; }
        public string? VehicleInfo { get; set; }
        public string? DriverInfo { get; set; }
        public decimal? FreightCost { get; set; }
        public decimal? InsuranceCost { get; set; }
        public decimal? OtherCosts { get; set; }
        public string? Notes { get; set; }
        public Guid? ReceivedByUserId { get; set; }
        public List<CreateGoodsReceiptItemRequest> Items { get; set; } = new();
    }

    public class CreateGoodsReceiptItemRequest
    {
        public Guid PurchaseOrderItemId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? WarehouseId { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal AcceptedQuantity { get; set; }
        public decimal RejectedQuantity { get; set; }
        public decimal DamagedQuantity { get; set; }
        public QualityStatus QualityStatus { get; set; }
        public string? QualityNotes { get; set; }
        public string? BatchNumber { get; set; }
        public string? SerialNumbers { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public string? StorageLocation { get; set; }
        public bool RequiresInspection { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateGoodsReceiptRequest
    {
        public DateTime ReceiptDate { get; set; }
        public string? DeliveryNote { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? TransportCompany { get; set; }
        public string? VehicleInfo { get; set; }
        public string? DriverInfo { get; set; }
        public decimal? FreightCost { get; set; }
        public decimal? InsuranceCost { get; set; }
        public decimal? OtherCosts { get; set; }
        public string? Notes { get; set; }
        public List<UpdateGoodsReceiptItemRequest> Items { get; set; } = new();
    }

    public class UpdateGoodsReceiptItemRequest
    {
        public Guid? Id { get; set; }
        public Guid PurchaseOrderItemId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? WarehouseId { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal AcceptedQuantity { get; set; }
        public decimal RejectedQuantity { get; set; }
        public decimal DamagedQuantity { get; set; }
        public QualityStatus QualityStatus { get; set; }
        public string? BatchNumber { get; set; }
        public string? SerialNumbers { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public string? StorageLocation { get; set; }
        public bool RequiresInspection { get; set; }
        public string? Notes { get; set; }
    }

    public class GoodsReceiptResponse
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public Guid PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public GoodsReceiptStatus Status { get; set; }
        public string? DeliveryNote { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? TransportCompany { get; set; }
        public string? VehicleInfo { get; set; }
        public string? DriverInfo { get; set; }
        public decimal? FreightCost { get; set; }
        public decimal? InsuranceCost { get; set; }
        public decimal? OtherCosts { get; set; }
        public decimal TotalCost { get; set; }
        public string? Notes { get; set; }
        public Guid? ReceivedByUserId { get; set; }
        public string? ReceivedByUserName { get; set; }
        public Guid? InspectedByUserId { get; set; }
        public string? InspectedByUserName { get; set; }
        public DateTime? InspectedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<GoodsReceiptItemResponse> Items { get; set; } = new();
    }

    public class GoodsReceiptItemResponse
    {
        public Guid Id { get; set; }
        public Guid GoodsReceiptId { get; set; }
        public Guid PurchaseOrderItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public Guid? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal AcceptedQuantity { get; set; }
        public decimal RejectedQuantity { get; set; }
        public decimal DamagedQuantity { get; set; }
        public QualityStatus QualityStatus { get; set; }
        public string? BatchNumber { get; set; }
        public string? SerialNumbers { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public string? StorageLocation { get; set; }
        public bool RequiresInspection { get; set; }
        public Guid? InspectedByUserId { get; set; }
        public string? InspectedByUserName { get; set; }
        public DateTime? InspectedAt { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class GoodsReceiptSummaryResponse
    {
        public Guid Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public GoodsReceiptStatus Status { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalCost { get; set; }
        public bool HasPendingInspections { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateGoodsReceiptStatusRequest
    {
        public GoodsReceiptStatus Status { get; set; }
        public string? Notes { get; set; }
    }

    public class InspectGoodsReceiptItemRequest
    {
        public Guid GoodsReceiptItemId { get; set; }
        public QualityStatus QualityStatus { get; set; }
        public decimal AcceptedQuantity { get; set; }
        public decimal RejectedQuantity { get; set; }
        public decimal DamagedQuantity { get; set; }
        public string? InspectionNotes { get; set; }
        public List<string>? DefectPhotos { get; set; }
    }

    public class BulkInspectionRequest
    {
        public List<InspectGoodsReceiptItemRequest> Items { get; set; } = new();
        public string? GeneralNotes { get; set; }
    }

    public class GoodsReceiptFilterRequest
    {
        public Guid? PurchaseOrderId { get; set; }
        public Guid? SupplierId { get; set; }
        public GoodsReceiptStatus? Status { get; set; }
        public QualityStatus? QualityStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? HasPendingInspections { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GoodsReceiptReportRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? SupplierId { get; set; }
        public Guid? WarehouseId { get; set; }
        public bool IncludeQualityMetrics { get; set; }
        public bool IncludeDefectAnalysis { get; set; }
    }

    public class GoodsReceiptReportResponse
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalReceipts { get; set; }
        public decimal TotalValue { get; set; }
        public int TotalItems { get; set; }
        public decimal AverageReceiptValue { get; set; }
        public QualityMetrics QualityMetrics { get; set; } = new();
        public List<SupplierPerformance> SupplierPerformances { get; set; } = new();
        public List<DefectAnalysis> DefectAnalyses { get; set; } = new();
    }

    public class QualityMetrics
    {
        public decimal AcceptanceRate { get; set; }
        public decimal RejectionRate { get; set; }
        public decimal DamageRate { get; set; }
        public int TotalInspections { get; set; }
        public int PassedInspections { get; set; }
        public int FailedInspections { get; set; }
    }

    public class SupplierPerformance
    {
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int TotalReceipts { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AcceptanceRate { get; set; }
        public decimal OnTimeDeliveryRate { get; set; }
        public decimal AverageQualityScore { get; set; }
    }

    public class DefectAnalysis
    {
        public string DefectType { get; set; } = string.Empty;
        public int Occurrences { get; set; }
        public decimal Percentage { get; set; }
        public decimal TotalValue { get; set; }
        public List<string> AffectedProducts { get; set; } = new();
    }
}