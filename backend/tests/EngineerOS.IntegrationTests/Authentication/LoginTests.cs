using System.Net;
using System.Net.Http.Json;

namespace EngineerOS.IntegrationTests.Authentication;

public sealed class LoginTests
    : IClassFixture<EngineerOSWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoginTests(
        EngineerOSWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnsOk()
    {
        var email =
            $"login-{Guid.NewGuid()}@example.com";

        var password =
            "TestPassword123!";

        await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                firstName = "Test",
                lastName = "User",
                email,
                password
            });

        var response =
            await _client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new
                {
                    email,
                    password
                });

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


// TEST 2: Wrong password
[Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var email =
            $"wrong-{Guid.NewGuid()}@example.com";

        await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                firstName = "Test",
                lastName = "User",
                email,
                password = "CorrectPassword123!"
            });

        var response =
            await _client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new
                {
                    email,
                    password = "WrongPassword123!"
                });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    // TEST 3: Non-existent user
    [Fact]
    public async Task Login_WithUnknownEmail_ReturnsUnauthorized()
    {
        var response =
            await _client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new
                {
                    email =
                        $"unknown-{Guid.NewGuid()}@example.com",

                    password =
                        "TestPassword123!"
                });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}