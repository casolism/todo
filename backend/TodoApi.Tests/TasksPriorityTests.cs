using System.Net;
using System.Net.Http.Json;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests;

public class TasksPriorityTests : IClassFixture<TestAppFactory>
{
    private readonly TestAppFactory _factory;

    public TasksPriorityTests(TestAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostTasks_WithInvalidPriority_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest("Tarea con prioridad inválida", "urgente"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostTasks_WithValidPriority_ReturnsCreatedWithPriority()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest("Tarea con prioridad alta", "alta"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.Equal("alta", created!.Priority);
    }
}
