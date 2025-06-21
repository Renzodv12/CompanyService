namespace CompanyService.Core.Entities
{
    public class Permission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Key { get; set; } // Ej: "Company", "User", "Role"
        public string Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
