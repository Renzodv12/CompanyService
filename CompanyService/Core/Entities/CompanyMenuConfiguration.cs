using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Entities
{
    public class CompanyMenuConfiguration
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        public Guid MenuId { get; set; }
        
        public bool IsEnabled { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public Company Company { get; set; } = null!;
        
        public Menu Menu { get; set; } = null!;
    }
}