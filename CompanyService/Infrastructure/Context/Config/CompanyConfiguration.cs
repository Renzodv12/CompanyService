using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.Context.Config
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.RUC).IsRequired().HasMaxLength(20);

            builder.Property(c => c.CreatedAt).HasDefaultValueSql("now()").IsRequired();
            builder.Property(c => c.LastModifiedAt).HasDefaultValueSql("now()").IsRequired();

            builder.HasMany(c => c.Roles).WithOne(r => r.Company).HasForeignKey(r => r.CompanyId);
            builder.HasMany(c => c.UserCompanies).WithOne(uc => uc.Company).HasForeignKey(uc => uc.CompanyId);
        }
    }

}
