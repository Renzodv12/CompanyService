using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
    {
        public void Configure(EntityTypeBuilder<StockMovement> builder)
        {
            builder.ToTable("StockMovements");
            builder.HasKey(sm => sm.Id);

            builder.Property(sm => sm.Reason).IsRequired().HasMaxLength(200);
            builder.Property(sm => sm.Reference).HasMaxLength(100);
            builder.Property(sm => sm.CreatedAt).HasDefaultValueSql("now()");
        }
    }
}
