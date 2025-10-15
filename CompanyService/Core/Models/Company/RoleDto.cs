using CompanyService.Core.Models.Company;

namespace CompanyService.Core.Models.Company
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new();
    }
}


