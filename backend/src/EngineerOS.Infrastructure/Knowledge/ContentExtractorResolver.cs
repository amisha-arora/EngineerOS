using EngineerOS.Application.Abstractions.Knowledge;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class ContentExtractorResolver
    : IContentExtractorResolver
{
    private readonly IReadOnlyList<IContentExtractor> _extractors;

    public ContentExtractorResolver(
        IEnumerable<IContentExtractor> extractors)
    {
        _extractors = extractors.ToList();
    }

    public IContentExtractor Resolve(string extension)
    {
        var extractor = _extractors.FirstOrDefault(
            extractor => extractor.CanExtract(extension));

        if (extractor is null)
        {
            throw new NotSupportedException(
                $"No content extractor is available for '{extension}'.");
        }

        return extractor;
    }
}