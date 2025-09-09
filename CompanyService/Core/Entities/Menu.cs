using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Entities
{
    public class Menu
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? Icon { get; set; }
        
        [MaxLength(200)]
        public string? Route { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public int? ParentId { get; set; }
        
        public int Order { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public Menu? Parent { get; set; }
        
        public ICollection<Menu> Children { get; set; } = new List<Menu>();
        
        public ICollection<CompanyMenuConfiguration> CompanyConfigurations { get; set; } = new List<CompanyMenuConfiguration>();
    }
}