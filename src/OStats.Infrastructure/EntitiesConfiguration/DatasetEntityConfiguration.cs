using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.Infrastructure.EntitiesConfiguration;

sealed class DatasetEntityConfiguration : EntityConfiguration<Dataset>
{
       public override void Configure(EntityTypeBuilder<Dataset> builder)
       {
              base.Configure(builder);

              builder.Property(dataset => dataset.Title)
                     .HasMaxLength(256);

              builder.Property(dataset => dataset.Source)
                     .HasMaxLength(256);

              builder.Property(dataset => dataset.Description)
                     .HasMaxLength(4096);

              builder.HasMany<DatasetProjectLink>()
                     .WithOne()
                     .HasForeignKey(link => link.DatasetId)
                     .IsRequired();

              builder.Navigation(dataset => dataset.DatasetUserAccessLevels).AutoInclude();
       }
}