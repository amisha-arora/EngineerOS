using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Persistence;
using EngineerOS.Application.Abstractions.Storage;

namespace EngineerOS.Application.Features.Repositories.GetFiles;

public sealed class GetRepositoryFilesService
{
    private readonly IRepositoryRepository _repositoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepositoryFileReader _repositoryFileReader;

    public GetRepositoryFilesService(
        IRepositoryRepository repositoryRepository,
        ICurrentUserService currentUserService,
        IRepositoryFileReader repositoryFileReader)
    {
        _repositoryRepository = repositoryRepository;
        _currentUserService = currentUserService;
        _repositoryFileReader = repositoryFileReader;
    }

    public async Task<GetRepositoryFilesResponse?> GetAsync(
        Guid repositoryId,
        CancellationToken cancellationToken)
    {
        var repository = await _repositoryRepository.GetByIdAsync(
            repositoryId,
            cancellationToken);

        if (repository is null ||
            repository.UserId != _currentUserService.UserId)
        {
            return null;
        }

        var files = await _repositoryFileReader.GetFilesAsync(
            repositoryId,
            cancellationToken);

        return new GetRepositoryFilesResponse(
            repositoryId,
            files);
    }
}