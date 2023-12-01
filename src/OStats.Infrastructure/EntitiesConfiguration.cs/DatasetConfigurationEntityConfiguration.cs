using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.Infrastructure.EntitiesConfiguration;

class DatasetConfigurationEntityConfiguration : EntityConfiguration<DatasetConfiguration>, IEntityTypeConfiguration<DatasetConfiguration>
{
    public void Configure(EntityTypeBuilder<DatasetConfiguration> builder)
    {
        BaseConfigure(builder);

        builder.Property(datasetConfig => datasetConfig.Title)
               .HasMaxLength(256);

        builder.Property(datasetConfig => datasetConfig.Source)
               .HasMaxLength(512);

        builder.Property(datasetConfig => datasetConfig.Description)
               .HasMaxLength(2048);
    }
}