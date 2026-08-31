using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Persistence;
using EngineerOS.Domain.Entities;

namespace EngineerOS.Application.Features.Repositories.Create;

public sealed class CreateRepositoryService
{
    private readonly IRepositoryRepository _repositoryRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateRepositoryService(
        IRepositoryRepository repositoryRepository,
        ICurrentUserService currentUserService)
    {
        _repositoryRepository = repositoryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CreateRepositoryResponse> CreateAsync(
        CreateRepositoryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var repository = new Repository(
            userId,
            request.Name.Trim(),
            request.Url.Trim());

        await _repositoryRepository.AddAsync(
            repository,
            cancellationToken);

        await _repositoryRepository.SaveChangesAsync(
            cancellationToken);

        return new CreateRepositoryResponse(
            repository.Id,
            repository.Name,
            repository.Url,
            repository.CreatedAtUtc);
    }
}

