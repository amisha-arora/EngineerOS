using System.Text;
using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Application.Knowledge.DTOs;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class RepositoryPromptContextBuilder
    : IRepositoryPromptContextBuilder
{
    // First version uses a character budget.
    // Later we can replace this with model-aware token counting.
    private const int MaxContextCharacters = 30_000;

    public string Build(RepositoryContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var builder = new StringBuilder();

        builder.AppendLine("REPOSITORY EVIDENCE");
        builder.AppendLine("===================");
        builder.AppendLine();

        foreach (var file in context.Files)
        {
            var fileSection = BuildFileSection(file);

            // Do not exceed our initial context budget.
            if (builder.Length + fileSection.Length >
                MaxContextCharacters)
            {
                break;
            }

            builder.Append(fileSection);
        }

        return builder.ToString();
    }

    private static string BuildFileSection(
        RepositoryContextFile file)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"FILE: {file.FileName}");
        builder.AppendLine($"PATH: {file.FilePath}");
        builder.AppendLine($"TYPE: {file.FileType}");
        builder.AppendLine(
            $"IMPORTANCE SCORE: {file.ImportanceScore}");

        builder.AppendLine(
            $"SEMANTIC SCORE: {file.SemanticScore:F4}");

        builder.AppendLine(
            $"GRAPH SCORE: {file.GraphScore:F4}");

        builder.AppendLine(
            $"RELEVANCE SCORE: {file.RelevanceScore:F4}");

        AppendList(
            builder,
            "CLASSES",
            file.Classes);

        AppendList(
            builder,
            "METHODS",
            file.Methods);

        AppendList(
            builder,
            "DEPENDENCIES",
            file.Dependencies);

        AppendList(
            builder,
            "DEPENDENTS",
            file.Dependents);

        if (file.RelevantChunks.Count > 0)
        {
            builder.AppendLine("RELEVANT CODE:");

            foreach (var chunk in file.RelevantChunks)
            {
                builder.AppendLine();

                if (!string.IsNullOrWhiteSpace(chunk.SymbolType) ||
                    !string.IsNullOrWhiteSpace(chunk.SymbolName))
                {
                    builder.AppendLine(
                        $"Symbol: {chunk.SymbolType} {chunk.SymbolName}");
                }

                if (chunk.StartLine.HasValue &&
                    chunk.EndLine.HasValue)
                {
                    builder.AppendLine(
                        $"Lines: {chunk.StartLine}-{chunk.EndLine}");
                }

                builder.AppendLine("```");
                builder.AppendLine(chunk.Content);
                builder.AppendLine("```");
            }
        }

        builder.AppendLine();
        builder.AppendLine("----------------------------------------");
        builder.AppendLine();

        return builder.ToString();
    }

    private static void AppendList(
        StringBuilder builder,
        string title,
        IReadOnlyList<string> values)
    {
        if (values.Count == 0)
        {
            return;
        }

        builder.AppendLine($"{title}:");

        foreach (var value in values)
        {
            builder.AppendLine($"- {value}");
        }
    }
}