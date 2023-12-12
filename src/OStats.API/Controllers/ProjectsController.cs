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

        if (project is null)
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
        return Ok(await _projectQueries.GetProjectUsersAndRoles(projectId));
    }

    [Route("{projectId:Guid}/datasetconfiguration")]
    [HttpGet]
    [ProducesResponseType(typeof(List<DatasetConfiguration>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<DatasetConfiguration>>> GetProjectDatasetsConfigurations(Guid projectId)
    {
        return Ok(await _projectQueries.GetDatasetsConfigurationsByProjectIdAsync(projectId));
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