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

    public UsersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Route("{userId:Guid}")]
    [HttpGet]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUserByIdAsync(Guid userId)
    {
        var authIdentity = User.Identity?.Name;
        if (authIdentity == null)
        {
            return BadRequest();
        }

        var query = new UserByIdQuery(authIdentity, userId);
        var queryResult = await _mediator.Send(query);

        if (!queryResult.Success)
        {
            return BadRequest(queryResult.ValidationFailures);
        }

        return Ok(queryResult.Value);
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

        var query = new UserByAuthIdQuery(authIdentity);
        var queryResult = await _mediator.Send(query);
        if (queryResult.Success)
        {
            return Ok(queryResult.Value);
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
        var authIdentity = User.Identity?.Name;
        if (authIdentity == null)
        {
            return BadRequest();
        }

        var query = new UserProjectsWithRoleQuery(authIdentity, userId);
        var queryResult = await _mediator.Send(query);

        if (!queryResult.Success)
        {
            return BadRequest(queryResult.ValidationFailures);
        }

        return Ok(queryResult.Value);
    }
}