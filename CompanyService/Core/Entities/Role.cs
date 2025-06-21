namespace CompanyService.Core.Entities
{
    public class Role
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<UserCompany> UserCompanies { get; set; } = new List<UserCompany>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    }

}
