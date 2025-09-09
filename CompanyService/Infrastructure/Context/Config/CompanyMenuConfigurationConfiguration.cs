using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class CompanyMenuConfigurationConfiguration : IEntityTypeConfiguration<CompanyMenuConfiguration>
    {
        public void Configure(EntityTypeBuilder<CompanyMenuConfiguration> builder)
        {
            builder.HasKey(cmc => cmc.Id);
            
            builder.Property(cmc => cmc.CompanyId)
                .IsRequired();
                
            builder.Property(cmc => cmc.MenuId)
                .IsRequired();
                
            builder.Property(cmc => cmc.IsEnabled)
                .IsRequired()
                .HasDefaultValue(true);
                
            builder.Property(cmc => cmc.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");
                
            builder.Property(cmc => cmc.UpdatedAt);
            
            // Relationships
            builder.HasOne(cmc => cmc.Company)
                .WithMany()
                .HasForeignKey(cmc => cmc.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne(cmc => cmc.Menu)
                .WithMany(m => m.CompanyConfigurations)
                .HasForeignKey(cmc => cmc.MenuId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Unique constraint to prevent duplicate configurations
            builder.HasIndex(cmc => new { cmc.CompanyId, cmc.MenuId })
                .IsUnique();
                
            // Indexes for better performance
            builder.HasIndex(cmc => cmc.CompanyId);
            builder.HasIndex(cmc => cmc.MenuId);
            builder.HasIndex(cmc => cmc.IsEnabled);
        }
    }
}