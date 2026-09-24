using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Persistence;

namespace EngineerOS.Application.Features.Repositories.GetDependencies;

public sealed class GetRepositoryDependenciesService
{
    private readonly IRepositoryRepository _repositoryRepository;
    private readonly IRepositoryAnalysisReader _repositoryAnalysisReader;
    private readonly ICurrentUserService _currentUserService;

    public GetRepositoryDependenciesService(
        IRepositoryRepository repositoryRepository,
        IRepositoryAnalysisReader repositoryAnalysisReader,
        ICurrentUserService currentUserService)
    {
        _repositoryRepository = repositoryRepository;
        _repositoryAnalysisReader = repositoryAnalysisReader;
        _currentUserService = currentUserService;
    }

    public async Task<GetRepositoryDependenciesResponse?> GetAsync(
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

        var dependencies =
            await _repositoryAnalysisReader.GetDependenciesAsync(
                repositoryId,
                cancellationToken);

        var responseDependencies = dependencies
            .Select(dependency =>
                new RepositoryDependencyResponse(
                    dependency.Id,
                    dependency.SourceCodeClassId,
                    dependency.SourceTypeName,
                    dependency.TargetCodeClassId,
                    dependency.TargetTypeName,
                    dependency.DependencyType,
                    dependency.LineNumber))
            .ToList();

        return new GetRepositoryDependenciesResponse(
            repositoryId,
            responseDependencies);
    }
}