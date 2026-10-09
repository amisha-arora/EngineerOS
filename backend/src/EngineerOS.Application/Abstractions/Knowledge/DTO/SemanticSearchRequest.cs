namespace EngineerOS.Application.Knowledge.DTOs;

public sealed class SemanticSearchRequest
{
    public string Query { get; set; } = string.Empty;

    public int TopK { get; set; } = 10;
}