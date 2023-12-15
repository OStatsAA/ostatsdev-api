using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OStats.API.Commands;
using OStats.API.Dtos;
using OStats.API.Queries;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Controllers;

[Route("v1/[controller]")]
[Authorize]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Route("{projectId:Guid}")]
    [HttpGet]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Project>> GetProjectByIdAsync(Guid projectId)
    {
        var authIdentity = User.Identity?.Name;
        if (authIdentity is null)
        {
            return BadRequest();
        }

        var query = new ProjectByIdQuery(authIdentity, projectId);
        var queryResult = await _mediator.Send(query);

        if (!queryResult.Success)
        {
            return BadRequest(queryResult.ValidationFailures);
        }

        return Ok(queryResult.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Project>> CreateProject([FromBody] CreateProjectCommandDto createDto)
    {
        var authIdentity = User.Identity?.Name;
        if (authIdentity is null)
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

    [HttpPut("{projectId:Guid}")]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Project>> UpdateProject(Guid projectId, [FromBody] UpdateProjectCommandDto updateDto)
    {
        var authIdentity = User.Identity?.Name;
        if (authIdentity is null)
        {
            return BadRequest();
        }

        var command = new UpdateProjectCommand(projectId, authIdentity, updateDto.Title, updateDto.LastUpdatedAt, updateDto.Description);
        var commandResult = await _mediator.Send(command);
        if (!commandResult.Success)
        {
            return BadRequest(commandResult.ValidationFailures);
        }

        return Ok(commandResult.Value);
    }

    [Route("{projectId:Guid}")]
    [HttpDelete]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> DeleteProject(Guid projectId)
    {
        var authIdentity = User.Identity?.Name;
        if (authIdentity is null)
        {
            return BadRequest();
        }

        var command = new DeleteProjectCommand(authIdentity, projectId);
        var commandResult = await _mediator.Send(command);
        if (!commandResult.Success)
        {
            return BadRequest(commandResult.ValidationFailures);
        }

        return Ok(commandResult.Value);
    }

    [Route("{projectId:Guid}/usersroles")]
    [HttpGet]
    [ProducesResponseType(typeof(List<ProjectUserAndRoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ProjectUserAndRoleDto>>> GetProjectUsersAndRoles(Guid projectId)
    {
        var authIdentity = User.Identity?.Name;
        if (authIdentity == null)
        {
            return BadRequest();
        }

        var query = new ProjectUsersAndRolesQuery(authIdentity, projectId);
        var queryResult = await _mediator.Send(query);

        if (!queryResult.Success)
        {
            return BadRequest(queryResult.ValidationFailures);
        }

        return Ok(queryResult.Value);
    }

    [Route("{projectId:Guid}/datasetconfiguration")]
    [HttpPost]
    [ProducesResponseType(typeof(DatasetConfiguration), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DatasetConfiguration>> AddDatasetConfigurationToProject(Guid projectId,
                                                                                           [FromBody] AddDatasetConfigurationCommandDto addDto)
    {
        var authIdentity = User.Identity?.Name;
        if (authIdentity is null)
        {
            return BadRequest();
        }

        var command = new AddDatasetConfigurationCommand(authIdentity, projectId, addDto.Title, addDto.Source, addDto.Description);
        var commandResult = await _mediator.Send(command);
        if (!commandResult.Success)
        {
            return BadRequest(commandResult.ValidationFailures);
        }

        return Ok(commandResult.Value);
    }

    [Route("{projectId:Guid}/datasetconfiguration/{datasetConfigId:Guid}")]
    [HttpDelete]
    [ProducesResponseType(typeof(DatasetConfiguration), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DatasetConfiguration>> DeleteDatasetConfigurationFromProject(Guid projectId, Guid datasetConfigId)
    {
        var authIdentity = User.Identity?.Name;
        if (authIdentity is null)
        {
            return BadRequest();
        }

        var command = new RemoveDatasetConfigurationCommand(authIdentity, projectId, datasetConfigId);
        var commandResult = await _mediator.Send(command);
        if (!commandResult.Success)
        {
            return BadRequest(commandResult.ValidationFailures);
        }

        return Ok(commandResult.Value);
    }

}