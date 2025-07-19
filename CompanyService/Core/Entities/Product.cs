using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; } // Código único del producto
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; } // Stock mínimo para alertas
        public int MaxStock { get; set; }
        public bool IsActive { get; set; } = true;
        public ProductType Type { get; set; } // Producto, Servicio
        public string Unit { get; set; } // Unidad de medida (kg, unidad, litro, etc.)
        public decimal Weight { get; set; }
        public string ImageUrl { get; set; }

        public Guid CategoryId { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public ProductCategory Category { get; set; }
        public Company Company { get; set; }
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
    }
}
