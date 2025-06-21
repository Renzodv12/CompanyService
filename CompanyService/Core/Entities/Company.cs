using StackExchange.Redis;
using System.Data;

namespace CompanyService.Core.Entities
{
    public class Company
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string RUC { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<UserCompany> UserCompanies { get; set; } = new List<UserCompany>();
    }

}
