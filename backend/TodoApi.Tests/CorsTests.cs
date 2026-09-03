using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TodoApi.Tests;

public class CorsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CorsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Request_FromLocalhost4200_IsNotBlockedByCors()
    {
        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/health");
        request.Headers.Add("Origin", "http://localhost:4200");

        var response = await client.SendAsync(request);

        Assert.True(response.IsSuccessStatusCode);
        Assert.True(response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values));
        Assert.Contains("http://localhost:4200", values!);
    }
}
