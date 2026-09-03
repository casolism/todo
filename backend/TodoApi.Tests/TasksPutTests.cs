using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests;

public class TasksPutTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TasksPutTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PutTasks_TogglesCompleted_WhenTaskExists()
    {
        var client = _factory.CreateClient();
        var postResponse = await client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest("Lavar el carro"));
        var created = await postResponse.Content.ReadFromJsonAsync<TaskItem>();

        var putResponse = await client.PutAsync($"/api/tasks/{created!.Id}", content: null);

        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
        var updated = await putResponse.Content.ReadFromJsonAsync<TaskItem>();
        Assert.True(updated!.Completed);
    }

    [Fact]
    public async Task PutTasks_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        var client = _factory.CreateClient();

        var response = await client.PutAsync("/api/tasks/999999", content: null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
