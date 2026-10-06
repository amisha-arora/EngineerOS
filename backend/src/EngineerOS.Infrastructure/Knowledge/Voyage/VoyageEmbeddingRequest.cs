using System.Text.Json.Serialization;

namespace EngineerOS.Infrastructure.Knowledge.Voyage;

internal sealed class VoyageEmbeddingRequest
{
    [JsonPropertyName("input")]
    public string Input { get; init; } = string.Empty;

    [JsonPropertyName("model")]
    public string Model { get; init; } = string.Empty;

    [JsonPropertyName("input_type")]
    public string InputType { get; init; } = "document";

    [JsonPropertyName("output_dimension")]
    public int OutputDimension { get; init; }
}