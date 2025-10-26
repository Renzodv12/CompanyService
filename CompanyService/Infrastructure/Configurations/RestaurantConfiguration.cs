using CompanyService.Core.Entities.Restaurant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Configurations
{
    public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> builder)
        {
            builder.HasKey(r => r.Id);
            
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(r => r.Description)
                .HasMaxLength(500);
                
            builder.Property(r => r.Address)
                .HasMaxLength(300);
                
            builder.Property(r => r.City)
                .HasMaxLength(100);
                
            builder.Property(r => r.Phone)
                .HasMaxLength(20);
                
            builder.Property(r => r.Email)
                .HasMaxLength(100);
                
            builder.Property(r => r.RUC)
                .HasMaxLength(20);

            // Relationships
            builder.HasOne(r => r.Company)
                .WithMany()
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.CreatedByUser)
                .WithMany()
                .HasForeignKey(r => r.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.Tables)
                .WithOne(t => t.Restaurant)
                .HasForeignKey(t => t.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Menus)
                .WithOne(m => m.Restaurant)
                .HasForeignKey(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Orders)
                .WithOne(o => o.Restaurant)
                .HasForeignKey(o => o.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class RestaurantTableConfiguration : IEntityTypeConfiguration<RestaurantTable>
    {
        public void Configure(EntityTypeBuilder<RestaurantTable> builder)
        {
            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.TableNumber)
                .IsRequired()
                .HasMaxLength(20);
                
            builder.Property(t => t.Name)
                .HasMaxLength(100);
                
            builder.Property(t => t.Location)
                .HasMaxLength(100);
                
            builder.Property(t => t.Description)
                .HasMaxLength(300);

            // Relationships
            builder.HasOne(t => t.Restaurant)
                .WithMany(r => r.Tables)
                .HasForeignKey(t => t.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Company)
                .WithMany()
                .HasForeignKey(t => t.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.CurrentOrder)
                .WithOne()
                .HasForeignKey<RestaurantTable>(t => t.CurrentOrderId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class RestaurantMenuConfiguration : IEntityTypeConfiguration<RestaurantMenu>
    {
        public void Configure(EntityTypeBuilder<RestaurantMenu> builder)
        {
            builder.HasKey(m => m.Id);
            
            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(m => m.Description)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(m => m.Restaurant)
                .WithMany(r => r.Menus)
                .HasForeignKey(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Company)
                .WithMany()
                .HasForeignKey(m => m.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.CreatedByUser)
                .WithMany()
                .HasForeignKey(m => m.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.MenuItems)
                .WithOne(mi => mi.Menu)
                .HasForeignKey(mi => mi.MenuId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class RestaurantMenuItemConfiguration : IEntityTypeConfiguration<RestaurantMenuItem>
    {
        public void Configure(EntityTypeBuilder<RestaurantMenuItem> builder)
        {
            builder.HasKey(mi => mi.Id);
            
            builder.Property(mi => mi.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(mi => mi.Description)
                .HasMaxLength(500);
                
            builder.Property(mi => mi.Category)
                .HasMaxLength(100);

            builder.Property(mi => mi.Price)
                .HasColumnType("decimal(18,2)");

            // Relationships
            builder.HasOne(mi => mi.Menu)
                .WithMany(m => m.MenuItems)
                .HasForeignKey(mi => mi.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(mi => mi.Product)
                .WithMany()
                .HasForeignKey(mi => mi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mi => mi.Company)
                .WithMany()
                .HasForeignKey(mi => mi.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(mi => mi.CreatedByUser)
                .WithMany()
                .HasForeignKey(mi => mi.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class RestaurantOrderConfiguration : IEntityTypeConfiguration<RestaurantOrder>
    {
        public void Configure(EntityTypeBuilder<RestaurantOrder> builder)
        {
            builder.HasKey(o => o.Id);
            
            builder.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(o => o.CustomerName)
                .HasMaxLength(200);
                
            builder.Property(o => o.CustomerPhone)
                .HasMaxLength(20);
                
            builder.Property(o => o.Notes)
                .HasMaxLength(500);
                
            builder.Property(o => o.SpecialInstructions)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(o => o.Restaurant)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Table)
                .WithMany()
                .HasForeignKey(o => o.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Sale)
                .WithMany()
                .HasForeignKey(o => o.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.CreatedByUser)
                .WithMany()
                .HasForeignKey(o => o.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.AssignedWaiter)
                .WithMany()
                .HasForeignKey(o => o.AssignedWaiterId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
