using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.Infrastructure.EntitiesConfiguration;

internal sealed class ProjectEntityConfiguration : EntityConfiguration<Project>
{
       public override void Configure(EntityTypeBuilder<Project> builder)
       {
              base.Configure(builder);

              builder.Property(project => project.Title)
                     .HasMaxLength(256);

              builder.Property(project => project.Description)
                     .HasMaxLength(2048);

              builder.HasMany<Role>()
                     .WithOne()
                     .HasForeignKey(role => role.ProjectId)
                     .IsRequired();

              builder.HasMany(project => project.LinkedDatasets)
                     .WithOne()
                     .HasForeignKey(link => link.ProjectId)
                     .IsRequired();

              builder.Navigation(project => project.LinkedDatasets).AutoInclude();
       }
}