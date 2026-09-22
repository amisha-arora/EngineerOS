using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Extraction;
using EngineerOS.Application.Abstractions.Persistence;

namespace EngineerOS.Application.Features.Repositories.GetCSharpTypes;

public sealed class GetRepositoryCSharpTypesService
{
    private readonly IRepositoryRepository _repositoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICSharpTypeExtractor _cSharpTypeExtractor;

    public GetRepositoryCSharpTypesService(
        IRepositoryRepository repositoryRepository,
        ICurrentUserService currentUserService,
        ICSharpTypeExtractor cSharpTypeExtractor)
    {
        _repositoryRepository = repositoryRepository;
        _currentUserService = currentUserService;
        _cSharpTypeExtractor = cSharpTypeExtractor;
    }

    public async Task<GetRepositoryCSharpTypesResponse?> GetAsync(
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

        var types = await _cSharpTypeExtractor.ExtractAsync(
            repositoryId,
            cancellationToken);

        return new GetRepositoryCSharpTypesResponse(
            repositoryId,
            types);
    }
}