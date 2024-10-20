using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.Infrastructure.EntitiesConfiguration;

internal sealed class DatasetUserAccessLevelEntityConfiguration : EntityConfiguration<DatasetUserAccessLevel>
{
    public override void Configure(EntityTypeBuilder<DatasetUserAccessLevel> builder)
    {
        base.Configure(builder);
        builder.HasIndex(dsUserAccessLevel => dsUserAccessLevel.DatasetId);
        builder.HasIndex(dsUserAccessLevel => dsUserAccessLevel.UserId);
    }
}