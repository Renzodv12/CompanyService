namespace CompanyService.Core.Entities
{
    public class ProductCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CompanyId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Company Company { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
