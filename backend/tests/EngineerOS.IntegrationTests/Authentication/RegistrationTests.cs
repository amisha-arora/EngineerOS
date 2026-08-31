using System.Net;
using System.Net.Http.Json;

namespace EngineerOS.IntegrationTests.Authentication;

public sealed class RegistrationTests
    : IClassFixture<EngineerOSWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegistrationTests(
        EngineerOSWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }


    // TEST 1: Valid registration should return 201 Created
    [Fact]
    public async Task Register_WithValidData_ReturnsCreated()
    {
        var email =
            $"test-{Guid.NewGuid()}@example.com";

        var request = new
        {
            firstName = "Test",
            lastName = "User",
            email,
            password = "TestPassword123!"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/v1/auth/register",
                request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }


    // TEST 2: Registering the same email twice should return 409 Conflict
    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflict()
    {
        var email =
            $"duplicate-{Guid.NewGuid()}@example.com";

        var request = new
        {
            firstName = "Test",
            lastName = "User",
            email,
            password = "TestPassword123!"
        };

        // First registration
        await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            request);

        // Try registering same email again
        var secondResponse =
            await _client.PostAsJsonAsync(
                "/api/v1/auth/register",
                request);

        Assert.Equal(
            HttpStatusCode.Conflict,
            secondResponse.StatusCode);
    }


    // TEST 3: Invalid registration should return 400 Bad Request
    [Fact]
    public async Task Register_WithInvalidData_ReturnsBadRequest()
    {
        var request = new
        {
            firstName = "",
            lastName = "",
            email = "not-an-email",
            password = "123"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/v1/auth/register",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}