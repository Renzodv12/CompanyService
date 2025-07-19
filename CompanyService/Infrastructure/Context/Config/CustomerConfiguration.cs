using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.Property(c => c.DocumentNumber).IsRequired().HasMaxLength(20);
            builder.Property(c => c.Email).HasMaxLength(100);
            builder.Property(c => c.Phone).HasMaxLength(20);
            builder.Property(c => c.Address).HasMaxLength(300);
            builder.Property(c => c.City).HasMaxLength(100);

            builder.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
            builder.Property(c => c.LastModifiedAt).HasDefaultValueSql("now()");

            builder.HasIndex(c => new { c.DocumentNumber, c.CompanyId }).IsUnique();

            builder.HasOne(c => c.Company).WithMany().HasForeignKey(c => c.CompanyId);
        }
    }
}
