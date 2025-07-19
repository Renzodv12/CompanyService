using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
            builder.Property(s => s.DocumentNumber).IsRequired().HasMaxLength(20);
            builder.Property(s => s.Email).HasMaxLength(100);
            builder.Property(s => s.Phone).HasMaxLength(20);
            builder.Property(s => s.Address).HasMaxLength(300);
            builder.Property(s => s.City).HasMaxLength(100);
            builder.Property(s => s.ContactPerson).HasMaxLength(100);

            builder.Property(s => s.CreatedAt).HasDefaultValueSql("now()");
            builder.Property(s => s.LastModifiedAt).HasDefaultValueSql("now()");

            builder.HasIndex(s => new { s.DocumentNumber, s.CompanyId }).IsUnique();

            builder.HasOne(s => s.Company).WithMany().HasForeignKey(s => s.CompanyId);
        }
    }
}
