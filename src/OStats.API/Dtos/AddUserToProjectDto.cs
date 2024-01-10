using System.ComponentModel.DataAnnotations;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Dtos;

public record AddUserToProjectDto
{
    [Required]
    public Guid UserId { get; init; }
    [Required]
    public AccessLevel AccessLevel { get; init; }
}