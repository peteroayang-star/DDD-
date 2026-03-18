using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DddTemplate.Domain.OperationLogs;

namespace DddTemplate.Infrastructure.EntityFramework.Configurations;

public class OperationLogConfiguration : IEntityTypeConfiguration<OperationLog>
{
    public void Configure(EntityTypeBuilder<OperationLog> builder)
    {
        builder.ToTable("OperationLogs");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.UserName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Module)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.OperationType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.RequestPath)
            .HasMaxLength(500);

        builder.Property(o => o.RequestMethod)
            .HasMaxLength(10);

        builder.Property(o => o.IpAddress)
            .HasMaxLength(50);

        builder.Property(o => o.IsSuccess)
            .IsRequired();

        builder.Property(o => o.OperatedAt)
            .IsRequired();

        builder.Property(o => o.ExecutionTime)
            .IsRequired();

        builder.Ignore(o => o.DomainEvents);
    }
}
