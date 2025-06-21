namespace CompanyService.Core.Entities
{
    public class UserCompany
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } 
        public Guid CompanyId { get; set; }
        public Guid RoleId { get; set; }

        public Company Company { get; set; }
        public Role Role { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }

}
