using System.ComponentModel.DataAnnotations;

namespace CompanyService.Core.DTOs.Menu
{
    public class MenuConfigurationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Route { get; set; }
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public bool IsEnabled { get; set; }
        public List<MenuConfigurationDto> Children { get; set; } = new List<MenuConfigurationDto>();
    }

    public class CompanyMenuConfigurationDto
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public List<MenuConfigurationDto> Menus { get; set; } = new List<MenuConfigurationDto>();
    }

    public class UpdateMenuConfigurationRequest
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

