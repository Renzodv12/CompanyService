using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class SaleDetailConfiguration : IEntityTypeConfiguration<SaleDetail>
    {
        public void Configure(EntityTypeBuilder<SaleDetail> builder)
        {
            builder.ToTable("SaleDetails");
            builder.HasKey(sd => sd.Id);

            builder.Property(sd => sd.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Property(sd => sd.Discount).HasColumnType("decimal(18,2)");
            builder.Property(sd => sd.Subtotal).HasColumnType("decimal(18,2)");

            builder.HasOne(sd => sd.Sale).WithMany(s => s.SaleDetails).HasForeignKey(sd => sd.SaleId);
            builder.HasOne(sd => sd.Product).WithMany(p => p.SaleDetails).HasForeignKey(sd => sd.ProductId);
        }
    }
}
