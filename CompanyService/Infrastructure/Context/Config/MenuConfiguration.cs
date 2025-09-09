using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class MenuConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.HasKey(m => m.Id);
            
            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(m => m.Icon)
                .HasMaxLength(50);
                
            builder.Property(m => m.Route)
                .HasMaxLength(200);
                
            builder.Property(m => m.Description)
                .HasMaxLength(500);
                
            builder.Property(m => m.Order)
                .IsRequired();
                
            builder.Property(m => m.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
                
            builder.Property(m => m.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");
                
            builder.Property(m => m.UpdatedAt);
            
            // Self-referencing relationship for parent-child menus
            builder.HasOne(m => m.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(m => m.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Index for better performance
            builder.HasIndex(m => m.ParentId);
            builder.HasIndex(m => m.Order);
            builder.HasIndex(m => m.IsActive);
        }
    }
}