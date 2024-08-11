using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.Infrastructure.EntitiesConfiguration;

sealed class RoleEntityConfiguration : EntityConfiguration<Role>, IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        BaseConfigure(builder);
        builder.HasIndex(role => role.ProjectId);
        builder.HasIndex(role => role.UserId);
    }
}