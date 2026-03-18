using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DddTemplate.Domain.Menus;

namespace DddTemplate.Infrastructure.EntityFramework.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("Menus");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Icon)
            .HasMaxLength(50);

        builder.Property(m => m.Path)
            .HasMaxLength(200);

        builder.Property(m => m.ParentId);

        builder.Property(m => m.SortOrder)
            .IsRequired();

        builder.Property(m => m.IsEnabled)
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Ignore(m => m.DomainEvents);
    }
}
