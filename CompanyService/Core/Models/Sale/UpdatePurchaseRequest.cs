using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Models.Sale
{
    /// <summary>
    /// Request para actualizar una compra
    /// </summary>
    public class UpdatePurchaseRequest
    {
        [Required]
        public Guid SupplierId { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }

        [Required]
        [StringLength(100)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un item")]
        public List<PurchaseDetailItem> Items { get; set; } = new();
    }
}

