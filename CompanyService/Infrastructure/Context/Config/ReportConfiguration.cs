using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name).IsRequired().HasMaxLength(200);
            builder.Property(r => r.Description).HasMaxLength(500);
            builder.Property(r => r.Parameters).HasColumnType("jsonb"); // Para PostgreSQL
            builder.Property(r => r.CreatedAt).HasDefaultValueSql("now()");

            builder.HasOne(r => r.Company).WithMany().HasForeignKey(r => r.CompanyId);
        }
    }
}
