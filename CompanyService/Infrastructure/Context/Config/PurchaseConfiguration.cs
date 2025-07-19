using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Purchases");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.PurchaseNumber).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Subtotal).HasColumnType("decimal(18,2)");
            builder.Property(p => p.TaxAmount).HasColumnType("decimal(18,2)");
            builder.Property(p => p.TotalAmount).HasColumnType("decimal(18,2)");
            builder.Property(p => p.InvoiceNumber).HasMaxLength(50);
            builder.Property(p => p.Notes).HasMaxLength(500);

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("now()");

            builder.HasIndex(p => new { p.PurchaseNumber, p.CompanyId }).IsUnique();

            builder.HasOne(p => p.Supplier).WithMany(s => s.Purchases).HasForeignKey(p => p.SupplierId);
            builder.HasOne(p => p.Company).WithMany().HasForeignKey(p => p.CompanyId);
            builder.HasMany(p => p.PurchaseDetails).WithOne(pd => pd.Purchase).HasForeignKey(pd => pd.PurchaseId);
        }
    }
}
