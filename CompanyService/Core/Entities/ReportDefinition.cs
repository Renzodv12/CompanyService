using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Entities
{
    public class ReportDefinition
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; } // Product, Sale, Customer, etc.
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        public Guid CreatedByUserId { get; set; }
        
        public bool IsActive { get; set; } = true;
        public bool IsShared { get; set; } = false; // Si otros usuarios pueden ejecutarlo
        
        public int Version { get; set; } = 1;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
        
        // Navegación
        public Company Company { get; set; }
        public ICollection<ReportField> Fields { get; set; } = new List<ReportField>();
        public ICollection<ReportFilter> Filters { get; set; } = new List<ReportFilter>();
        public ICollection<ReportExecution> Executions { get; set; } = new List<ReportExecution>();
    }
}