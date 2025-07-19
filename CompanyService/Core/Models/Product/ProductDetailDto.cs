namespace CompanyService.Core.Models.Product
{
    public class ProductDetailDto : ProductDto
    {
        public decimal Weight { get; set; }
        public string ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public List<StockMovementDto> RecentMovements { get; set; } = new();
    }
}
