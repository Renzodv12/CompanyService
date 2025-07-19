using MediatR;

namespace CompanyService.Core.Feature.Commands.Product
{
    public class CreateProductCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; }
        public int MaxStock { get; set; }
        public string Unit { get; set; }
        public decimal Weight { get; set; }
        public Guid CategoryId { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
