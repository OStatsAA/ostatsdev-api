using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.Infrastructure.EntitiesConfiguration;

sealed class DatasetProjectLinkConfiguration : EntityConfiguration<DatasetProjectLink>
{
    public override void Configure(EntityTypeBuilder<DatasetProjectLink> builder)
    {
        base.Configure(builder);
        builder.HasIndex(role => role.DatasetId);
        builder.HasIndex(role => role.ProjectId);
    }
}