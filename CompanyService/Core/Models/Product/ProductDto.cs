using CompanyService.Core.Enums;

namespace CompanyService.Core.Models.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; }
        public int MaxStock { get; set; }
        public bool IsActive { get; set; }
        public ProductType Type { get; set; }
        public string Unit { get; set; }
        public string CategoryName { get; set; }
        public bool IsLowStock => Stock <= MinStock;
    }
}
