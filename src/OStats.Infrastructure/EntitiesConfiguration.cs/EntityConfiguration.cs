using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OStats.Domain.Common;

namespace OStats.Infrastructure.EntitiesConfiguration;

class EntityConfiguration<T> where T : Entity
{
    public void BaseConfigure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(entity => entity.Id);
    }
}