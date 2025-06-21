using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.Context.Config
{
    public class UserCompanyConfiguration : IEntityTypeConfiguration<UserCompany>
    {
        public void Configure(EntityTypeBuilder<UserCompany> builder)
        {
            builder.ToTable("UserCompanies");
            builder.HasKey(uc => uc.Id);

            builder.Property(uc => uc.UserId).IsRequired();
            builder.Property(uc => uc.AssignedAt).IsRequired().HasDefaultValueSql("now()");

            builder.HasOne(uc => uc.Company)
                   .WithMany(c => c.UserCompanies)
                   .HasForeignKey(uc => uc.CompanyId);

            builder.HasOne(uc => uc.Role)
                   .WithMany(r => r.UserCompanies)
                   .HasForeignKey(uc => uc.RoleId);


        }
    }

}
