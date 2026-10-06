using EngineerOS.Domain.Entities;

namespace EngineerOS.Application.Abstractions.RepositoryFiles;

public interface IRepositoryFileReader
{
    Task<IReadOnlyList<RepositoryFile>> GetFilesAsync(
        Guid repositoryId,
        CancellationToken cancellationToken);
}