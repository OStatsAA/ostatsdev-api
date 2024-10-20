using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Infrastructure.EntitiesConfiguration;

internal sealed class UserEntityConfiguration : EntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(user => user.Name).HasMaxLength(128);

        builder.Property(user => user.Email).HasMaxLength(128);

        builder.HasIndex(user => user.AuthIdentity).IsUnique();

        builder.HasMany<Role>()
               .WithOne()
               .HasForeignKey(role => role.UserId)
               .IsRequired();

        builder.HasMany<DatasetUserAccessLevel>()
               .WithOne()
               .HasForeignKey(datasetAccess => datasetAccess.UserId)
               .IsRequired();
    }
}