using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class PurchaseDetailConfiguration : IEntityTypeConfiguration<PurchaseDetail>
    {
        public void Configure(EntityTypeBuilder<PurchaseDetail> builder)
        {
            builder.ToTable("PurchaseDetails");
            builder.HasKey(pd => pd.Id);

            builder.Property(pd => pd.UnitCost).HasColumnType("decimal(18,2)");
            builder.Property(pd => pd.Subtotal).HasColumnType("decimal(18,2)");

            builder.HasOne(pd => pd.Purchase).WithMany(p => p.PurchaseDetails).HasForeignKey(pd => pd.PurchaseId);
            builder.HasOne(pd => pd.Product).WithMany(p => p.PurchaseDetails).HasForeignKey(pd => pd.ProductId);
        }
    }
}
