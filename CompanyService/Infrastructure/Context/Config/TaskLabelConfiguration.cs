using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class TaskLabelConfiguration : IEntityTypeConfiguration<TaskLabel>
    {
        public void Configure(EntityTypeBuilder<TaskLabel> builder)
        {
            builder.ToTable("TaskLabels");
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Name).IsRequired().HasMaxLength(50);
            builder.Property(l => l.Color).HasMaxLength(7).HasDefaultValue("#3B82F6");

            // Relaciones
            builder.HasOne(l => l.Task).WithMany(t => t.TaskLabels).HasForeignKey(l => l.TaskId);
        }
    }
}
