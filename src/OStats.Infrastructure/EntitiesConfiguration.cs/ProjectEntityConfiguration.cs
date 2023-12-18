using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.Infrastructure.EntitiesConfiguration;

class ProjectEntityConfiguration : EntityConfiguration<Project>, IEntityTypeConfiguration<Project>
{
       public void Configure(EntityTypeBuilder<Project> builder)
       {
              BaseConfigure(builder);

              builder.Property(project => project.Title)
                     .HasMaxLength(256);

              builder.Property(project => project.Description)
                     .HasMaxLength(2048);

              builder.HasMany(project => project.Roles)
                     .WithOne()
                     .HasForeignKey(role => role.ProjectId)
                     .IsRequired();

              builder.HasMany(project => project.LinkedDatasets)
                     .WithOne()
                     .HasForeignKey(link => link.ProjectId)
                     .IsRequired();

              builder.Navigation(project => project.Roles).AutoInclude();
              builder.Navigation(project => project.LinkedDatasets).AutoInclude();
       }
}