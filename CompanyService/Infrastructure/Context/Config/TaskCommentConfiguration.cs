using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
    {
        public void Configure(EntityTypeBuilder<TaskComment> builder)
        {
            builder.ToTable("TaskComments");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.AuthorName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.AuthorUsername).IsRequired().HasMaxLength(50);
            builder.Property(c => c.AuthorAvatar).HasMaxLength(500);
            builder.Property(c => c.Content).IsRequired().HasMaxLength(2000);
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("now()");

            // Relaciones
            builder.HasOne(c => c.Task).WithMany(t => t.TaskComments).HasForeignKey(c => c.TaskId);
            builder.HasOne(c => c.ParentComment).WithMany(c => c.Replies).HasForeignKey(c => c.ParentCommentId);

            // Índices
            builder.HasIndex(c => new { c.TaskId, c.CreatedAt });
        }
    }
}
