using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests;

public class TasksDeleteTests : IClassFixture<TestAppFactory>
{
    private readonly TestAppFactory _factory;

    public TasksDeleteTests(TestAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeleteTasks_RemovesTask_WhenTaskExists()
    {
        var client = _factory.CreateClient();
        var postResponse = await client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest("Sacar la basura"));
        var created = await postResponse.Content.ReadFromJsonAsync<TaskItem>();

        var deleteResponse = await client.DeleteAsync($"/api/tasks/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync("/api/tasks");
        var tasks = await getResponse.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.DoesNotContain(tasks!, t => t.Id == created.Id);
    }

    [Fact]
    public async Task DeleteTasks_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        var client = _factory.CreateClient();

        var response = await client.DeleteAsync("/api/tasks/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
