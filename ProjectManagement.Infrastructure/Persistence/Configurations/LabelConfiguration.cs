using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Core.Entities;

namespace ProjectManagement.Infrastructure.Persistence.Configurations;

public class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Name).IsRequired().HasMaxLength(50);
        builder.Property(l => l.Color).IsRequired().HasMaxLength(7);
        builder.HasOne(l => l.Project)
            .WithMany(p => p.Labels)
            .HasForeignKey(l => l.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(l => l.TaskLabels)
            .WithOne(tl => tl.Label)
            .HasForeignKey(tl => tl.LabelId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(l => !l.IsDeleted);
    }
}
