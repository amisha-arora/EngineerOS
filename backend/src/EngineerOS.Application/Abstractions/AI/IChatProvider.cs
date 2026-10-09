namespace EngineerOS.Application.Abstractions.AI;

public interface IChatProvider
{
    Task<string> GenerateAnswerAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default);
}