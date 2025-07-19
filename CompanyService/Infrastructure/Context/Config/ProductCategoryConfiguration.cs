using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable("ProductCategories");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(300);
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("now()");

            builder.HasIndex(c => new { c.Name, c.CompanyId }).IsUnique();

            builder.HasOne(c => c.Company).WithMany().HasForeignKey(c => c.CompanyId);
        }
    }
}
