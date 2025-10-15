namespace CompanyService.Core.Models.Company
{
    public class CreateRoleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
    }

    public class UpdateRoleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class AssignPermissionsRequest
    {
        public List<Guid> PermissionIds { get; set; } = new();
    }

    public class AssignPermissionsWithActionsRequest
    {
        public List<PermissionAssignment> Permissions { get; set; } = new();
    }

    public class PermissionAssignment
    {
        public Guid PermissionId { get; set; }
        public List<string> Actions { get; set; } = new();
    }
}
