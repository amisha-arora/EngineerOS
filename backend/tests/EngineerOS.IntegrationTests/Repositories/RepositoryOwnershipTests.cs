using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EngineerOS.IntegrationTests.Repositories;

public sealed class RepositoryOwnershipTests
    : IClassFixture<EngineerOSWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RepositoryOwnershipTests(
        EngineerOSWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private sealed record LoginResponse(
        string AccessToken,
        string RefreshToken);

    private sealed record RepositoryResponse(
        Guid Id,
        string Name,
        string Url);

    private async Task<string> RegisterAndLoginAsync(
        string email)
    {
        const string password = "TestPassword123!";

        await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                firstName = "Test",
                lastName = "User",
                email,
                password
            });

        var loginResponse =
            await _client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new
                {
                    email,
                    password
                });

        var login =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        return login!.AccessToken;
    }

    [Fact]
    public async Task GetRepository_WhenOwnedByAnotherUser_ReturnsNotFound()
    {
        var userAToken =
            await RegisterAndLoginAsync(
                $"user-a-{Guid.NewGuid()}@example.com");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userAToken);

        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/v1/repositories",
                new
                {
                    name = "Repository X",
                    url = $"https://github.com/test/{Guid.NewGuid()}"
                });

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var repository =
            await createResponse.Content
                .ReadFromJsonAsync<RepositoryResponse>();

        Assert.NotNull(repository);

        var userBToken =
            await RegisterAndLoginAsync(
                $"user-b-{Guid.NewGuid()}@example.com");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userBToken);

        var response =
            await _client.GetAsync(
                $"/api/v1/repositories/{repository.Id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task DeleteRepository_WhenOwnedByAnotherUser_ReturnsNotFound()
    {
        var userAToken =
            await RegisterAndLoginAsync(
                $"user-a-{Guid.NewGuid()}@example.com");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userAToken);

        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/v1/repositories",
                new
                {
                    name = "Repository X",
                    url = $"https://github.com/test/{Guid.NewGuid()}"
                });

        var repository =
            await createResponse.Content
                .ReadFromJsonAsync<RepositoryResponse>();

        Assert.NotNull(repository);

        var userBToken =
            await RegisterAndLoginAsync(
                $"user-b-{Guid.NewGuid()}@example.com");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userBToken);

        var response =
            await _client.DeleteAsync(
                $"/api/v1/repositories/{repository.Id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task DeleteRepository_WhenOwnedByCurrentUser_ReturnsNoContent()
    {
        var userAToken =
            await RegisterAndLoginAsync(
                $"user-a-{Guid.NewGuid()}@example.com");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userAToken);

        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/v1/repositories",
                new
                {
                    name = "Repository X",
                    url = $"https://github.com/test/{Guid.NewGuid()}"
                });

        var repository =
            await createResponse.Content
                .ReadFromJsonAsync<RepositoryResponse>();

        Assert.NotNull(repository);

        var response =
            await _client.DeleteAsync(
                $"/api/v1/repositories/{repository.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }
}