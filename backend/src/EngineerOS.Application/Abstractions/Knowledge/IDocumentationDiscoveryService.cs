using EngineerOS.Application.Knowledge.DTOs;

namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IDocumentationDiscoveryService
{
    Task<IReadOnlyList<DocumentDto>> DiscoverAsync(
        Guid repositoryId,
        CancellationToken cancellationToken = default);
}