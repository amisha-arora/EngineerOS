using EngineerOS.Application.Knowledge.DTOs;

namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IRepositoryPromptContextBuilder
{
    string Build(RepositoryContext context);
}