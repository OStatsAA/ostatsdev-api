using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.Infrastructure.EntitiesConfiguration;

class DatasetUserAccessLevelEntityConfiguration : EntityConfiguration<DatasetUserAccessLevel>, IEntityTypeConfiguration<DatasetUserAccessLevel>
{
    public void Configure(EntityTypeBuilder<DatasetUserAccessLevel> builder)
    {
        BaseConfigure(builder);
        builder.HasIndex(dsUserAccessLevel => dsUserAccessLevel.DatasetId);
        builder.HasIndex(dsUserAccessLevel => dsUserAccessLevel.UserId);
    }
}