using CompanyService.Core.Enums;

namespace CompanyService.Core.Models.Company
{
    public class CompanyWithPermissionsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string RUC { get; set; }
        public string Role { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new();
    }


    public class PermissionDto
    {
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Actions { get; set; }
    }

}
