using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Context.Config
{
    public class ReportExecutionConfiguration : IEntityTypeConfiguration<ReportExecution>
    {
        public void Configure(EntityTypeBuilder<ReportExecution> builder)
        {
            builder.ToTable("ReportExecutions");
            builder.HasKey(re => re.Id);

            builder.Property(re => re.Parameters).HasColumnType("jsonb");
            builder.Property(re => re.ResultData).HasColumnType("jsonb");
            builder.Property(re => re.FilePath).HasMaxLength(500);
            builder.Property(re => re.ExecutionDate).HasDefaultValueSql("now()");

            builder.HasIndex(re => new { re.ReportId, re.ExecutionDate });

            builder.HasOne(re => re.Report).WithMany(r => r.ReportExecutions).HasForeignKey(re => re.ReportId);
        }
    }
} 
