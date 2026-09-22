using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Extraction;
using EngineerOS.Application.Abstractions.Persistence;

namespace EngineerOS.Application.Features.Repositories.GetCSharpMethods;

public sealed class GetRepositoryCSharpMethodsService
{
    private readonly IRepositoryRepository _repositoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICSharpMethodExtractor _cSharpMethodExtractor;

    public GetRepositoryCSharpMethodsService(
        IRepositoryRepository repositoryRepository,
        ICurrentUserService currentUserService,
        ICSharpMethodExtractor cSharpMethodExtractor)
    {
        _repositoryRepository = repositoryRepository;
        _currentUserService = currentUserService;
        _cSharpMethodExtractor = cSharpMethodExtractor;
    }

    public async Task<GetRepositoryCSharpMethodsResponse?> GetAsync(
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

        var methods = await _cSharpMethodExtractor.ExtractAsync(
            repositoryId,
            cancellationToken);

        return new GetRepositoryCSharpMethodsResponse(
            repositoryId,
            methods);
    }
}