using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace EngineerOS.IntegrationTests.Authentication;

public sealed class ProtectedEndpointTests
    : IClassFixture<EngineerOSWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProtectedEndpointTests(
        EngineerOSWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private sealed record LoginResponse(
        string AccessToken,
        string RefreshToken);


    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        var response =
            await _client.GetAsync(
                "/api/v1/test/protected");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }


    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_ReturnsOk()
    {
        var email =
            $"protected-{Guid.NewGuid()}@example.com";

        var password =
            "TestPassword123!";

        // 1. Register a user
        await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                firstName = "Test",
                lastName = "User",
                email,
                password
            });

        // 2. Login with that user
        var loginResponse =
            await _client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new
                {
                    email,
                    password
                });

        // 3. Read accessToken from login response
        var login =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        // 4. Put token into Authorization header
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login!.AccessToken);

        // 5. Call protected endpoint
        var response =
            await _client.GetAsync(
                "/api/v1/test/protected");

        // 6. Expect 200 OK
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }
}