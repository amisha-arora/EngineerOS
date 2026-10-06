using System.Text.Json.Serialization;

namespace EngineerOS.Infrastructure.Knowledge.Voyage;

internal sealed class VoyageEmbeddingResponse
{
    [JsonPropertyName("data")]
    public List<VoyageEmbeddingData> Data { get; init; } = [];

    [JsonPropertyName("model")]
    public string Model { get; init; } = string.Empty;
}

internal sealed class VoyageEmbeddingData
{
    [JsonPropertyName("embedding")]
    public List<float> Embedding { get; init; } = [];

    [JsonPropertyName("index")]
    public int Index { get; init; }
}