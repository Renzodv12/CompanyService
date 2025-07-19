using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
    {
        public void Configure(EntityTypeBuilder<JournalEntry> builder)
        {
            builder.ToTable("JournalEntries");
            builder.HasKey(je => je.Id);

            builder.Property(je => je.EntryNumber).IsRequired().HasMaxLength(50);
            builder.Property(je => je.DebitAmount).HasColumnType("decimal(18,2)");
            builder.Property(je => je.CreditAmount).HasColumnType("decimal(18,2)");
            builder.Property(je => je.Description).IsRequired().HasMaxLength(300);
            builder.Property(je => je.Reference).HasMaxLength(100);

            builder.Property(je => je.CreatedAt).HasDefaultValueSql("now()");
        }
    }
 }
