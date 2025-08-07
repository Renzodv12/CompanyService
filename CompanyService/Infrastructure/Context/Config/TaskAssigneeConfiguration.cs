using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class TaskAssigneeConfiguration : IEntityTypeConfiguration<TaskAssignee>
    {
        public void Configure(EntityTypeBuilder<TaskAssignee> builder)
        {
            builder.ToTable("TaskAssignees");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Username).IsRequired().HasMaxLength(50);
            builder.Property(a => a.Avatar).HasMaxLength(500);
            builder.Property(a => a.AssignedAt).HasDefaultValueSql("now()");

            // Relaciones
            builder.HasOne(a => a.Task).WithMany(t => t.TaskAssignees).HasForeignKey(a => a.TaskId);

            // Índices únicos - un usuario no puede estar asignado dos veces a la misma tarea
            builder.HasIndex(a => new { a.TaskId, a.UserId }).IsUnique();
        }
    }
}
