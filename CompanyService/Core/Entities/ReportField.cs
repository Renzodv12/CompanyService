using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Entities
{
    public class ReportField
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public Guid ReportDefinitionId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FieldName { get; set; } // Nombre del campo en la entidad (ej: "Name", "CreatedAt")
        
        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } // Nombre para mostrar (ej: "Nombre del Producto")
        
        [Required]
        [MaxLength(50)]
        public string DataType { get; set; } // string, int, decimal, datetime, bool
        
        public int DisplayOrder { get; set; }
        
        public bool IsVisible { get; set; } = true;
        
        [MaxLength(50)]
        public string? AggregateFunction { get; set; } // SUM, COUNT, AVG, MIN, MAX
        
        [MaxLength(100)]
        public string? FormatString { get; set; } // Para formateo de fechas, números, etc.
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navegación
        public ReportDefinition ReportDefinition { get; set; }
    }
}