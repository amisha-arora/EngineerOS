using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Persistence;

namespace EngineerOS.Application.Features.Repositories.Delete;

public sealed class DeleteRepositoryService
{
    private readonly IRepositoryRepository _repositoryRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteRepositoryService(
        IRepositoryRepository repositoryRepository,
        ICurrentUserService currentUserService)
    {
        _repositoryRepository = repositoryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> DeleteAsync(
        Guid repositoryId,
        CancellationToken cancellationToken)
    {
        var repository =
            await _repositoryRepository.GetByIdAsync(
                repositoryId,
                cancellationToken);

        if (repository is null)
        {
            return false;
        }

        if (repository.UserId != _currentUserService.UserId)
        {
            return false;
        }

        await _repositoryRepository.DeleteAsync(
            repository,
            cancellationToken);

        await _repositoryRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}