namespace EngineerOS.Application.Abstractions.Knowledge;

public sealed record EmbeddingResult(
    IReadOnlyList<float> Vector,
    string Model);