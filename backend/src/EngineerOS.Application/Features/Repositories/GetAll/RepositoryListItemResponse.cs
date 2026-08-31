namespace EngineerOS.Application.Features.Repositories.GetAll;

public sealed record RepositoryListItemResponse(
    Guid Id,
    string Name,
    string Url,
    DateTime CreatedAtUtc);

