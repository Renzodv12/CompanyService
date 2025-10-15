using CompanyService.Core.Enums;

namespace CompanyService.Core.Models.Sale
{
    /// <summary>
    /// DTO detallado para una compra específica
    /// </summary>
    public class PurchaseDetailDto
    {
        public Guid Id { get; set; }
        public string PurchaseNumber { get; set; } = string.Empty;
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public PurchaseStatus Status { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public List<PurchaseItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// DTO para items de compra
    /// </summary>
    public class PurchaseItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}

