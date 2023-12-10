using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OStats.API.Commands;
using OStats.API.Dtos;
using OStats.API.Queries;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Controllers;

[Route("v1/[controller]")]
[Authorize]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserQueries _userQueries;

    public UsersController(IMediator mediator, IUserQueries userQueries)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
    }

    [Route("{userId:Guid}")]
    [HttpGet]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUserByIdAsync(Guid userId)
    {
        var project = await _userQueries.GetUserByIdAsync(userId);

        if (project == null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserCommandDto createDto)
    {
        var authIdentity = User.Identity?.Name;
        if (authIdentity == null)
        {
            return BadRequest();
        }

        var registeredUser = await _userQueries.GetUserByAuthIdentity(authIdentity);
        if (registeredUser is not null)
        {
            return Ok(registeredUser);
        }

        var command = new CreateUserCommand(createDto.Name, createDto.Email, authIdentity);
        var commandResult = await _mediator.Send(command);
        if (!commandResult.Success)
        {
            return BadRequest(commandResult.ValidationFailures);
        }

        return Ok(commandResult.Value);
    }

    [Route("{userId:Guid}/projects")]
    [HttpGet]
    [ProducesResponseType(typeof(List<UserProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserProjectDto>>> GetProjectsByUserIdAsync(Guid userId)
    {
        return Ok(await _userQueries.GetProjectsByUserIdAsync(userId));
    }
}