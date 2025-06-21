using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.Entities;

namespace CompanyService.Infrastructure.Context.Config
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name).IsRequired().HasMaxLength(50);

            builder.HasOne(r => r.Company)
                   .WithMany(c => c.Roles)
                   .HasForeignKey(r => r.CompanyId);

            builder.HasMany(r => r.UserCompanies)
                   .WithOne(uc => uc.Role)
                   .HasForeignKey(uc => uc.RoleId);

            builder.HasMany(r => r.RolePermissions)
               .WithOne(rp => rp.Role)
               .HasForeignKey(rp => rp.RoleId)
               .OnDelete(DeleteBehavior.Cascade);

        }
    }

}
