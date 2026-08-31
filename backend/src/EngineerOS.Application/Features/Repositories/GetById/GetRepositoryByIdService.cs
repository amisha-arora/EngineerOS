using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Persistence;

namespace EngineerOS.Application.Features.Repositories.GetById;

public sealed class GetRepositoryByIdService
{
    private readonly IRepositoryRepository _repositoryRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetRepositoryByIdService(
        IRepositoryRepository repositoryRepository,
        ICurrentUserService currentUserService)
    {
        _repositoryRepository = repositoryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetRepositoryResponse?> GetAsync(
        Guid repositoryId,
        CancellationToken cancellationToken)
    {
        var repository =
            await _repositoryRepository.GetByIdAsync(
                repositoryId,
                cancellationToken);

        if (repository is null)
        {
            return null;
        }

        if (repository.UserId != _currentUserService.UserId)
        {
            return null;
        }

        return new GetRepositoryResponse(
            repository.Id,
            repository.Name,
            repository.Url,
            repository.CreatedAtUtc);
    }
}

