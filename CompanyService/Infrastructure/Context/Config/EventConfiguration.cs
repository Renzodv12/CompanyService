using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Description).HasMaxLength(1000);
            builder.Property(e => e.Start).IsRequired();
            builder.Property(e => e.End).IsRequired();
            builder.Property(e => e.AllDay).HasDefaultValue(false);
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            builder.Property(e => e.LastModifiedAt).HasDefaultValueSql("now()");

            // Relaciones
            builder.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyId);
            builder.HasMany(e => e.EventAttendees).WithOne(ea => ea.Event).HasForeignKey(ea => ea.EventId);

            // Índices
            builder.HasIndex(e => new { e.CompanyId, e.Start });
            builder.HasIndex(e => new { e.CompanyId, e.End });
            builder.HasIndex(e => e.CreatedBy);
        }
    }

    public class EventAttendeeConfiguration : IEntityTypeConfiguration<EventAttendee>
    {
        public void Configure(EntityTypeBuilder<EventAttendee> builder)
        {
            builder.ToTable("EventAttendees");
            builder.HasKey(ea => ea.Id);

            builder.Property(ea => ea.IsOrganizer).HasDefaultValue(false);
            builder.Property(ea => ea.CreatedAt).HasDefaultValueSql("now()");

            // Relaciones
            builder.HasOne(ea => ea.Event).WithMany(e => e.EventAttendees).HasForeignKey(ea => ea.EventId);

            // Índices únicos - un usuario no puede estar duplicado en un evento
            builder.HasIndex(ea => new { ea.EventId, ea.UserId }).IsUnique();
        }
    }
}