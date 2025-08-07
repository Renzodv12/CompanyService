using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class TaskColumnConfiguration : IEntityTypeConfiguration<TaskColumn>
    {
        public void Configure(EntityTypeBuilder<TaskColumn> builder)
        {
            builder.ToTable("TaskColumns");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("now()");

            // Relaciones
            builder.HasOne(c => c.Company).WithMany().HasForeignKey(c => c.CompanyId);
            builder.HasMany(c => c.Tasks).WithOne(t => t.Column).HasForeignKey(t => t.ColumnId);

            // Índices
            builder.HasIndex(c => new { c.CompanyId, c.DisplayOrder });
        }
    }
}
