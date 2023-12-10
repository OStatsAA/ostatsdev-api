using OStats.API.Dtos;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Queries;

public interface IUserQueries
{
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User?> GetUserByNameAsync(string name);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByAuthIdentity(string authIdentity);
    Task<List<UserProjectDto>> GetProjectsByUserIdAsync(Guid userId);
}