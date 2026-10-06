using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Application.Abstractions.RepositoryFiles;
using EngineerOS.Application.Knowledge.DTOs;

namespace EngineerOS.Application.Features.Repositories.Knowledge;

public sealed class DocumentationDiscoveryService
    : IDocumentationDiscoveryService
{
    private readonly IRepositoryFileReader _fileReader;
    private readonly IDocumentationDetector _documentationDetector;

    public DocumentationDiscoveryService(
        IRepositoryFileReader fileReader,
        IDocumentationDetector documentationDetector)
    {
        _fileReader = fileReader;
        _documentationDetector = documentationDetector;
    }

    public async Task<IReadOnlyList<DocumentDto>> DiscoverAsync(
        Guid repositoryId,
        CancellationToken cancellationToken = default)
    {
        var files = await _fileReader.GetFilesAsync(
            repositoryId,
            cancellationToken);

        var documents = files
            .Where(file =>
                _documentationDetector.IsDocumentation(file.Extension))
            .Select(file => new DocumentDto
            {
                Id = file.Id,
                Name = file.Name,
                Path = file.Path,
                Type = default
            })
            .ToList();

        return documents;
    }
}