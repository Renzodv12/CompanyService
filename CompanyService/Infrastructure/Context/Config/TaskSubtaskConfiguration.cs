using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class TaskSubtaskConfiguration : IEntityTypeConfiguration<TaskSubtask>
    {
        public void Configure(EntityTypeBuilder<TaskSubtask> builder)
        {
            builder.ToTable("TaskSubtasks");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Title).IsRequired().HasMaxLength(200);
            builder.Property(s => s.IsCompleted).HasDefaultValue(false);
            builder.Property(s => s.CreatedAt).HasDefaultValueSql("now()");

            // Relaciones
            builder.HasOne(s => s.Task).WithMany(t => t.TaskSubtasks).HasForeignKey(s => s.TaskId);

            // Índices
            builder.HasIndex(s => new { s.TaskId, s.DisplayOrder });
        }
    }

}
