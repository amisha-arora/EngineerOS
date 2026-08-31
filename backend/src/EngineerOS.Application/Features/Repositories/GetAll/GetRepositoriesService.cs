using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Persistence;

namespace EngineerOS.Application.Features.Repositories.GetAll;

public sealed class GetRepositoriesService
{
    private readonly IRepositoryRepository _repositoryRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetRepositoriesService(
        IRepositoryRepository repositoryRepository,
        ICurrentUserService currentUserService)
    {
        _repositoryRepository = repositoryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyCollection<RepositoryListItemResponse>> GetAsync(
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var repositories =
            await _repositoryRepository.GetByUserIdAsync(
                userId,
                cancellationToken);

        return repositories
            .Select(repository =>
                new RepositoryListItemResponse(
                    repository.Id,
                    repository.Name,
                    repository.Url,
                    repository.CreatedAtUtc))
            .ToList();
    }
}

