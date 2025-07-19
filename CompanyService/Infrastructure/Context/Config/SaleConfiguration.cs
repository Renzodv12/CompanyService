using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.SaleNumber).IsRequired().HasMaxLength(50);
            builder.Property(s => s.Subtotal).HasColumnType("decimal(18,2)");
            builder.Property(s => s.TaxAmount).HasColumnType("decimal(18,2)");
            builder.Property(s => s.DiscountAmount).HasColumnType("decimal(18,2)");
            builder.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)");
            builder.Property(s => s.Notes).HasMaxLength(500);
            builder.Property(s => s.ElectronicInvoiceId).HasMaxLength(100);

            builder.Property(s => s.CreatedAt).HasDefaultValueSql("now()");

            builder.HasIndex(s => new { s.SaleNumber, s.CompanyId }).IsUnique();

            // Relaciones
            builder.HasOne(s => s.Customer).WithMany(c => c.Sales).HasForeignKey(s => s.CustomerId);
            builder.HasOne(s => s.Company).WithMany().HasForeignKey(s => s.CompanyId);
            builder.HasMany(s => s.SaleDetails).WithOne(sd => sd.Sale).HasForeignKey(sd => sd.SaleId);
        }
    }
}
