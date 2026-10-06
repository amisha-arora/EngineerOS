using System.Net.Http.Headers;
using System.Net.Http.Json;
using EngineerOS.Application.Abstractions.Knowledge;
using Microsoft.Extensions.Options;

namespace EngineerOS.Infrastructure.Knowledge.Voyage;

public sealed class VoyageEmbeddingProvider
    : IEmbeddingProvider
{
    private readonly HttpClient _httpClient;
    private readonly VoyageOptions _options;

    public VoyageEmbeddingProvider(
        HttpClient httpClient,
        IOptions<VoyageOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<EmbeddingResult> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Text cannot be empty.",
                nameof(text));
        }

        var request = new VoyageEmbeddingRequest
        {
            Input = text,
            Model = _options.Model,
            InputType = "document",
            OutputDimension = _options.Dimensions
        };

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            "v1/embeddings");

        message.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _options.ApiKey);

        message.Content =
            JsonContent.Create(request);

        using var response =
            await _httpClient.SendAsync(
                message,
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            throw new HttpRequestException(
                $"Voyage embedding request failed " +
                $"with status {(int)response.StatusCode}: {error}");
        }

        var result =
            await response.Content
                .ReadFromJsonAsync<VoyageEmbeddingResponse>(
                    cancellationToken);

        if (result is null ||
            result.Data.Count == 0)
        {
            throw new InvalidOperationException(
                "Voyage returned no embedding.");
        }

        var vector = result.Data[0].Embedding;

        if (vector.Count != _options.Dimensions)
        {
            throw new InvalidOperationException(
                $"Expected {_options.Dimensions} dimensions " +
                $"but Voyage returned {vector.Count}.");
        }

        return new EmbeddingResult(
            vector,
            result.Model);
    }
}