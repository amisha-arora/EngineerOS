namespace EngineerOS.Application.Features.Repositories.Create;

public sealed record CreateRepositoryResponse(
    Guid Id,
    string Name,
    string Url,
    DateTime CreatedAtUtc);

