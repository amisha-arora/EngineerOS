namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IContentExtractorResolver
{
    IContentExtractor Resolve(string extension);
}