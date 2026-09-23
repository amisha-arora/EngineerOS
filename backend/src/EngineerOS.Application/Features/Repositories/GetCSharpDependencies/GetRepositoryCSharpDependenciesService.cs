using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Extraction;
using EngineerOS.Application.Abstractions.Persistence;

namespace EngineerOS.Application.Features.Repositories.GetCSharpDependencies;

public sealed class GetRepositoryCSharpDependenciesService
{
    private readonly IRepositoryRepository _repositoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICSharpDependencyExtractor _cSharpDependencyExtractor;

    public GetRepositoryCSharpDependenciesService(
        IRepositoryRepository repositoryRepository,
        ICurrentUserService currentUserService,
        ICSharpDependencyExtractor cSharpDependencyExtractor)
    {
        _repositoryRepository = repositoryRepository;
        _currentUserService = currentUserService;
        _cSharpDependencyExtractor = cSharpDependencyExtractor;
    }

    public async Task<GetRepositoryCSharpDependenciesResponse?> GetAsync(
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

        var dependencies = await _cSharpDependencyExtractor.ExtractAsync(
            repositoryId,
            cancellationToken);

        return new GetRepositoryCSharpDependenciesResponse(
            repositoryId,
            dependencies);
    }
}