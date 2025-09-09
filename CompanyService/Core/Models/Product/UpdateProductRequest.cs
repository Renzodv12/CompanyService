namespace CompanyService.Core.Models.Product
{
    public class UpdateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public int MinStock { get; set; }
        public int MaxStock { get; set; }
        public string Unit { get; set; }
        public decimal Weight { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsActive { get; set; }
        public string? Barcode { get; set; }
        public string? ImageUrl { get; set; }
    }
}
