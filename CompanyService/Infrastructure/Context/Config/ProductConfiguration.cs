using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.SKU).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Barcode).HasMaxLength(50);
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Property(p => p.Cost).HasColumnType("decimal(18,2)");
            builder.Property(p => p.Weight).HasColumnType("decimal(10,3)");
            builder.Property(p => p.Unit).HasMaxLength(20);
            builder.Property(p => p.ImageUrl).HasMaxLength(500);

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
            builder.Property(p => p.LastModifiedAt).HasDefaultValueSql("now()");

            builder.HasIndex(p => new { p.SKU, p.CompanyId }).IsUnique();
            builder.HasIndex(p => new { p.Barcode, p.CompanyId }).IsUnique();

            // Relaciones
            builder.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId);
            builder.HasOne(p => p.Company).WithMany().HasForeignKey(p => p.CompanyId);
        }
    }
}
