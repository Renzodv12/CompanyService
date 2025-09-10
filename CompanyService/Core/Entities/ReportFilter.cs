using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Entities
{
    public class ReportFilter
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public Guid ReportDefinitionId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FieldName { get; set; } // Campo a filtrar
        
        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } // Nombre para mostrar
        
        [Required]
        [MaxLength(50)]
        public string DataType { get; set; } // string, int, decimal, datetime, bool
        
        [Required]
        [MaxLength(20)]
        public string Operator { get; set; } // equals, contains, greater_than, less_than, between, in
        
        public string? DefaultValue { get; set; } // Valor por defecto del filtro
        
        public bool IsRequired { get; set; } = false;
        public bool IsUserEditable { get; set; } = true; // Si el usuario puede modificar este filtro
        
        public int DisplayOrder { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navegación
        public ReportDefinition ReportDefinition { get; set; }
    }
}