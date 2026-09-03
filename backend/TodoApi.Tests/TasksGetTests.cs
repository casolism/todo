using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TodoApi.Tests;

public class TasksGetTests : IClassFixture<TestAppFactory>
{
    private readonly TestAppFactory _factory;

    public TasksGetTests(TestAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetTasks_ReturnsOkWithJsonArray()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.StartsWith("[", body.Trim());
    }
}
