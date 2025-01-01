using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OStats.Domain.Common;

namespace OStats.Infrastructure.EntitiesConfiguration;

internal abstract class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : Entity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.CreatedAt).IsRequired();
        builder.Property(entity => entity.LastUpdatedAt).IsRequired();
        
        builder.Property(entity => entity.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasIndex(entity => entity.IsDeleted).HasFilter("\"IsDeleted\" = false");
        builder.HasQueryFilter(entity => !entity.IsDeleted);
    }
}