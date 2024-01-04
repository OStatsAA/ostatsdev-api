using OStats.Domain.Common;

namespace OStats.API.Dtos;

public record BaseEntityDto
{
    public Guid Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime LastUpdatedAt { get; }
    public BaseEntityDto(Entity entity)
    {
        Id = entity.Id;
        CreatedAt = entity.CreatedAt;
        LastUpdatedAt = entity.LastUpdatedAt;
    }
}