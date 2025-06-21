using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }


        public DbSet<Company> Users => Set<Company>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserCompany> UserCompanys => Set<UserCompany>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<Permission> Permissions => Set<Permission>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);


        }
    }
}
