using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class TaskConfiguration : IEntityTypeConfiguration<CompanyService.Core.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<CompanyService.Core.Entities.Task> builder)
        {
            builder.ToTable("Tasks");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Description).HasMaxLength(2000);
            builder.Property(t => t.CreatedAt).HasDefaultValueSql("now()");
            builder.Property(t => t.LastModifiedAt).HasDefaultValueSql("now()");

            // Relaciones
            builder.HasOne(t => t.Column).WithMany(c => c.Tasks).HasForeignKey(t => t.ColumnId);
            builder.HasOne(t => t.Company).WithMany().HasForeignKey(t => t.CompanyId);
            builder.HasMany(t => t.TaskLabels).WithOne(l => l.Task).HasForeignKey(l => l.TaskId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(t => t.TaskAssignees).WithOne(a => a.Task).HasForeignKey(a => a.TaskId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(t => t.TaskAttachments).WithOne(a => a.Task).HasForeignKey(a => a.TaskId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(t => t.TaskSubtasks).WithOne(s => s.Task).HasForeignKey(s => s.TaskId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(t => t.TaskComments).WithOne(c => c.Task).HasForeignKey(c => c.TaskId).OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(t => new { t.CompanyId, t.ColumnId });
            builder.HasIndex(t => t.CreatedAt);
        }
    }

}
