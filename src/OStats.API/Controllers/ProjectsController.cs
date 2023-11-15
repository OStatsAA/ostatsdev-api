using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OStats.API.Commands;
using OStats.API.Queries;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Controllers;

[Route("v1/[controller]")]
[Authorize]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IProjectQueries _projectQueries;

    public ProjectsController(IMediator mediator, IProjectQueries projectQueries)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _projectQueries = projectQueries ?? throw new ArgumentNullException(nameof(projectQueries));
    }

    [Route("{projectId:Guid}")]
    [HttpGet]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Project>> GetProjectByIdAsync(Guid projectId)
    {
        var project = await _projectQueries.GetProjectByIdAsync(projectId);

        if (project == null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Project>> CreateProject([FromBody] CreateProjectCommandDto createDto)
    {
        var authIdentity = User.Identity?.Name;
        if (authIdentity == null)
        {
            return BadRequest();
        }

        var command = new CreateProjectCommand(authIdentity, createDto.Title, createDto.Description);
        var commandResult = await _mediator.Send(command);
        if (!commandResult.Success)
        {
            return BadRequest(commandResult.ValidationFailures);
        }

        return Ok(commandResult.Value);
    }
}