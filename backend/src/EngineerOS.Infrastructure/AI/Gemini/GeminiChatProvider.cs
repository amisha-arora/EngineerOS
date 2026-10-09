using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EngineerOS.Application.Abstractions.AI;
using Microsoft.Extensions.Options;

namespace EngineerOS.Infrastructure.AI.Gemini;

public sealed class GeminiChatProvider : IChatProvider
{
    private readonly HttpClient _httpClient;
    private readonly GeminiOptions _options;

    public GeminiChatProvider(
        HttpClient httpClient,
        IOptions<GeminiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> GenerateAnswerAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException(
                "Gemini API key is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_options.Model))
        {
            throw new InvalidOperationException(
                "Gemini model is not configured.");
        }

        var requestBody = new
        {
            systemInstruction = new
            {
                parts = new[]
                {
                    new { text = systemPrompt }
                }
            },
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = userPrompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.2
            }
        };

        var endpoint =
            $"v1beta/models/{Uri.EscapeDataString(_options.Model)}:generateContent";

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            endpoint);

        request.Headers.Add(
            "x-goog-api-key",
            _options.ApiKey);

        request.Content = JsonContent.Create(requestBody);

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(
                cancellationToken);

            // Temporary diagnostic logging for local development only.
            Console.WriteLine(
                $"Gemini HTTP {(int)response.StatusCode}: {errorBody}");

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new HttpRequestException(
                    "Gemini rate limit or quota exceeded.",
                    null,
                    response.StatusCode);
            }

            throw new HttpRequestException(
                $"Gemini API request failed with status {(int)response.StatusCode}.",
                null,
                response.StatusCode);
        }

        using var responseStream =
            await response.Content.ReadAsStreamAsync(
                cancellationToken);

        using var json = await JsonDocument.ParseAsync(
            responseStream,
            cancellationToken: cancellationToken);

        var root = json.RootElement;

        if (!root.TryGetProperty("candidates", out var candidates)
            || candidates.GetArrayLength() == 0)
        {
            throw new InvalidOperationException(
                "Gemini returned no answer candidates.");
        }

        var candidate = candidates[0];

        if (!candidate.TryGetProperty("content", out var content)
            || !content.TryGetProperty("parts", out var parts))
        {
            throw new InvalidOperationException(
                "Gemini returned no answer content.");
        }

        var answerParts = new List<string>();

        foreach (var part in parts.EnumerateArray())
        {
            if (part.TryGetProperty("text", out var textElement))
            {
                var text = textElement.GetString();

                if (!string.IsNullOrWhiteSpace(text))
                {
                    answerParts.Add(text);
                }
            }
        }

        if (answerParts.Count == 0)
        {
            throw new InvalidOperationException(
                "Gemini returned an empty answer.");
        }

        return string.Join("\n", answerParts);
    }
}