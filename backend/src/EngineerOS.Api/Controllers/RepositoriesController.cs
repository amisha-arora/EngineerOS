using EngineerOS.Application.Features.Repositories.Create;
using EngineerOS.Application.Features.Repositories.GetAll;
using EngineerOS.Application.Features.Repositories.GetById;
using EngineerOS.Application.Features.Repositories.Delete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EngineerOS.Api.Controllers;

[ApiController]
[Route("api/v1/repositories")]
[Authorize]
public sealed class RepositoriesController : ControllerBase
{
    private readonly CreateRepositoryService _createRepositoryService;
    private readonly GetRepositoriesService _getRepositoriesService;
    private readonly GetRepositoryByIdService _getRepositoryByIdService;
    private readonly DeleteRepositoryService _deleteRepositoryService;

    public RepositoriesController(
        CreateRepositoryService createRepositoryService,
        GetRepositoriesService getRepositoriesService,
        GetRepositoryByIdService getRepositoryByIdService,
        DeleteRepositoryService deleteRepositoryService)
    {
        _createRepositoryService = createRepositoryService;
        _getRepositoriesService = getRepositoriesService;
        _getRepositoryByIdService = getRepositoryByIdService;
        _deleteRepositoryService = deleteRepositoryService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateRepositoryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _createRepositoryService.CreateAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var response = await _getRepositoriesService.GetAsync(cancellationToken);
        return Ok(response);
    }

    [HttpGet("{repositoryId:guid}")]
    public async Task<IActionResult> GetById(
        Guid repositoryId,
        CancellationToken cancellationToken)
    {
        var response = await _getRepositoryByIdService.GetAsync(repositoryId, cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpDelete("{repositoryId:guid}")]
    public async Task<IActionResult> Delete(
        Guid repositoryId,
        CancellationToken cancellationToken)
    {
        var deleted = await _deleteRepositoryService.DeleteAsync(
            repositoryId,
            cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
