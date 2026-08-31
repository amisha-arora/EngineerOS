namespace EngineerOS.Application.Features.Repositories.GetById;

public sealed record GetRepositoryResponse(
    Guid Id,
    string Name,
    string Url,
    DateTime CreatedAtUtc);

