using System.Net;
using System.Net.Http.Json;

namespace DisputePortal.IntegrationTests;

public class AuthEndpointTests : IClassFixture<DisputePortalFactory>
{
    private readonly DisputePortalFactory _factory;
    public AuthEndpointTests(DisputePortalFactory factory) => _factory = factory;

    [Fact]
    public async Task Login_WithSeededCustomer_ReturnsToken()
    {
        var client = _factory.CreateClient();   

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { email = "joe@email.com", password = "Password123!" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { email = "joe@email.com", password = "wrong" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private record LoginResponse(string Token);
}