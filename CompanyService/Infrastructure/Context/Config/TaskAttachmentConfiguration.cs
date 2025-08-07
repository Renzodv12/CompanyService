using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class TaskAttachmentConfiguration : IEntityTypeConfiguration<TaskAttachment>
    {
        public void Configure(EntityTypeBuilder<TaskAttachment> builder)
        {
            builder.ToTable("TaskAttachments");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name).IsRequired().HasMaxLength(255);
            builder.Property(a => a.Extension).IsRequired().HasMaxLength(10);
            builder.Property(a => a.Url).IsRequired().HasMaxLength(500);
            builder.Property(a => a.Size).HasMaxLength(20);
            builder.Property(a => a.UploadedAt).HasDefaultValueSql("now()");

            // Relaciones
            builder.HasOne(a => a.Task).WithMany(t => t.TaskAttachments).HasForeignKey(a => a.TaskId);
        }
    }
}
