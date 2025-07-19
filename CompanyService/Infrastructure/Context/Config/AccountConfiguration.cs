using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Code).IsRequired().HasMaxLength(20);
            builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
            builder.Property(a => a.Description).HasMaxLength(500);
            builder.Property(a => a.CreatedAt).HasDefaultValueSql("now()");

            builder.HasIndex(a => new { a.Code, a.CompanyId }).IsUnique();

            // Relaciones
            builder.HasOne(a => a.ParentAccount).WithMany(a => a.SubAccounts).HasForeignKey(a => a.ParentAccountId);
            builder.HasOne(a => a.Company).WithMany().HasForeignKey(a => a.CompanyId);
        }
    }
}
