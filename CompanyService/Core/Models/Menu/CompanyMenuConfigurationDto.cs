namespace CompanyService.Core.Models.Menu
{
    public class CompanyMenuConfigurationDto
    {
        public int Id { get; set; }
        public Guid CompanyId { get; set; }
        public int MenuId { get; set; }
        public bool IsEnabled { get; set; }
        public MenuDto Menu { get; set; } = null!;
    }
}