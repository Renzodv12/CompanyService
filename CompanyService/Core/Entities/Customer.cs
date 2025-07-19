using CompanyService.Core.Enums;
using System.Xml.Linq;

namespace CompanyService.Core.Entities
{
    public class Customer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string DocumentNumber { get; set; } // RUC o CI
        public DocumentType DocumentType { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CompanyId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Company Company { get; set; }
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
