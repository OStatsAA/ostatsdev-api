using System.ComponentModel.DataAnnotations;
using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.API.Dtos;

public record AddUserToDatasetDto
{
    [Required]
    public Guid UserId { get; init; }
    [Required]
    public DatasetAccessLevel AccessLevel { get; init; }
}