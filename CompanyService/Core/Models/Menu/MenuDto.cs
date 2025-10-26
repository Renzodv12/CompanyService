namespace CompanyService.Core.Models.Menu
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Route { get; set; }
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public List<MenuDto> Children { get; set; } = new List<MenuDto>();
    }
}