namespace CompanyService.Core.Models.Menu
{
    public class CompanyMenuConfigurationDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid MenuId { get; set; }
        public bool IsEnabled { get; set; }
        public MenuDto Menu { get; set; } = null!;
    }
}