using System.Text.Json.Serialization;
using OStats.Domain.Common;

namespace OStats.API.Dtos;

public record BaseEntityDto
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastUpdatedAt { get; init; }
    public BaseEntityDto(Entity entity)
    {
        Id = entity.Id;
        CreatedAt = entity.CreatedAt;
        LastUpdatedAt = entity.LastUpdatedAt;
    }

    [JsonConstructor]
    public BaseEntityDto(Guid id, DateTime createdAt, DateTime lastUpdatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        LastUpdatedAt = lastUpdatedAt;
    }
}