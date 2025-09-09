using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.Models.Menu
{
    public class UpdateCompanyMenuConfigurationRequest
    {
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        public List<MenuConfigurationItem> MenuConfigurations { get; set; } = new List<MenuConfigurationItem>();
    }
    
    public class MenuConfigurationItem
    {
        [Required]
        public int MenuId { get; set; }
        
        [Required]
        public bool IsEnabled { get; set; }
    }
}