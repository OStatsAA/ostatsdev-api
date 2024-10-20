using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OStats.Domain.Common;

namespace OStats.Infrastructure.EntitiesConfiguration;

sealed class AggregateHistoryEntryConfiguration : IEntityTypeConfiguration<AggregateHistoryEntry>
{
    public void Configure(EntityTypeBuilder<AggregateHistoryEntry> builder)
    {
        builder.HasKey(entry => entry.Id);
        builder.HasIndex(entry => entry.AggregateId);
        builder.HasIndex(entry => entry.UserId);
    }
}
