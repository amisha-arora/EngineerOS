using EngineerOS.Application.Abstractions.Knowledge;

namespace EngineerOS.Application.Knowledge.DTOs;

public sealed record RepositoryContext(
	Guid RepositoryId,
	string Query,
	IReadOnlyList<RepositoryContextFile> Files);