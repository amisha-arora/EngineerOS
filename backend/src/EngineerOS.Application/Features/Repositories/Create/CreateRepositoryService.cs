using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Extraction;
using EngineerOS.Application.Abstractions.Persistence;
using EngineerOS.Application.Abstractions.RepositoryFiles;
using EngineerOS.Domain.Entities;

namespace EngineerOS.Application.Features.Repositories.Create;

public sealed class CreateRepositoryService
{
    private readonly IRepositoryRepository _repositoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepositoryFileStorage _repositoryFileStorage;
    private readonly IRepositoryExtractor _repositoryExtractor;

    public CreateRepositoryService(
        IRepositoryRepository repositoryRepository,
        ICurrentUserService currentUserService,
        IRepositoryFileStorage repositoryFileStorage,
        IRepositoryExtractor repositoryExtractor)
    {
        _repositoryRepository = repositoryRepository;
        _currentUserService = currentUserService;
        _repositoryFileStorage = repositoryFileStorage;
        _repositoryExtractor = repositoryExtractor;
    }

    public async Task<CreateRepositoryResponse> CreateAsync(
        CreateRepositoryRequest request,
        CancellationToken cancellationToken)
    {
        var repository = new Repository(
            _currentUserService.UserId,
            request.Name.Trim(),
            request.Url.Trim());

        await _repositoryRepository.AddAsync(repository, cancellationToken);
         

        await _repositoryFileStorage.SaveAsync(
            repository.Id,
            request.FileStream,
            request.FileName,
            cancellationToken);

        await _repositoryExtractor.ExtractAsync(
            repository.Id,
            cancellationToken);

        return new CreateRepositoryResponse(
            repository.Id,
            repository.Name,
            repository.Url,
            repository.CreatedAtUtc);
    }
}