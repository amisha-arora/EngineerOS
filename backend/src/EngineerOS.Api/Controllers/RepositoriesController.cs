using EngineerOS.Application.Features.Repositories.Create;
using EngineerOS.Application.Features.Repositories.GetAll;
using EngineerOS.Application.Features.Repositories.GetById;
using EngineerOS.Application.Features.Repositories.Delete;
using EngineerOS.Application.Features.Repositories.GetCSharpTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EngineerOS.Application.Features.Repositories.GetFiles;
namespace EngineerOS.Api.Controllers;

using System.IO;

[ApiController]
[Route("api/v1/repositories")]
[Authorize]
public sealed class RepositoriesController : ControllerBase
{
    private readonly CreateRepositoryService _createRepositoryService;
    private readonly GetRepositoriesService _getRepositoriesService;
    private readonly GetRepositoryByIdService _getRepositoryByIdService;
    private readonly DeleteRepositoryService _deleteRepositoryService;
    private readonly GetRepositoryFilesService _getRepositoryFilesService;
    private readonly GetRepositoryCSharpTypesService _getRepositoryCSharpTypesService;

    public RepositoriesController(
        CreateRepositoryService createRepositoryService,
        GetRepositoriesService getRepositoriesService,
        GetRepositoryByIdService getRepositoryByIdService,
        DeleteRepositoryService deleteRepositoryService,
        GetRepositoryFilesService getRepositoryFilesService,
        GetRepositoryCSharpTypesService getRepositoryCSharpTypesService)
    {
        _createRepositoryService = createRepositoryService;
        _getRepositoriesService = getRepositoriesService;
        _getRepositoryByIdService = getRepositoryByIdService;
        _deleteRepositoryService = deleteRepositoryService;
        _getRepositoryFilesService = getRepositoryFilesService;
        _getRepositoryCSharpTypesService = getRepositoryCSharpTypesService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
    [FromForm] string name,
    [FromForm] string url,
    IFormFile file,
    CancellationToken cancellationToken)
    {
        await using var fileStream = file.OpenReadStream();

        var request = new CreateRepositoryRequest(
            name,
            url,
            fileStream,
            file.FileName);
        try
        {
            var response = await _createRepositoryService.CreateAsync(
                request,
                cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                response);
        }
        catch (InvalidDataException exception)
        {
            return BadRequest(new
            {
                error = exception.Message
            });
        }
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

    [HttpGet("{repositoryId:guid}/files")]
    public async Task<IActionResult> GetFiles(
    Guid repositoryId,
    CancellationToken cancellationToken)
    {
        var response = await _getRepositoryFilesService.GetAsync(
            repositoryId,
            cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }
    [HttpGet("{repositoryId:guid}/csharp-types")]
    public async Task<IActionResult> GetCSharpTypes(
    Guid repositoryId,
    CancellationToken cancellationToken)
    {
        var response = await _getRepositoryCSharpTypesService.GetAsync(
            repositoryId,
            cancellationToken);

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
