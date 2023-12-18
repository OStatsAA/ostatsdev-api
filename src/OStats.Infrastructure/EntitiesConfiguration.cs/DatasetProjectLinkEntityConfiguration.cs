using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.Infrastructure.EntitiesConfiguration;

class DatasetProjectLinkConfiguration : EntityConfiguration<DatasetProjectLink>, IEntityTypeConfiguration<DatasetProjectLink>
{
    public void Configure(EntityTypeBuilder<DatasetProjectLink> builder)
    {
        BaseConfigure(builder);
        builder.HasIndex(role => role.DatasetId);
        builder.HasIndex(role => role.ProjectId);
    }
}