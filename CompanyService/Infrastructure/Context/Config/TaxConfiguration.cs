using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class TaxConfiguration : IEntityTypeConfiguration<Tax>
    {
        public void Configure(EntityTypeBuilder<Tax> builder)
        {
            builder.ToTable("Taxes");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Rate).HasColumnType("decimal(5,4)"); // Para porcentajes como 0.1000 (10%)
            builder.Property(t => t.CreatedAt).HasDefaultValueSql("now()");

            builder.HasIndex(t => new { t.Name, t.CompanyId }).IsUnique();

            builder.HasOne(t => t.Company).WithMany().HasForeignKey(t => t.CompanyId);
        }
    }
}
