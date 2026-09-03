using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests;

public class TasksPostTests : IClassFixture<TestAppFactory>
{
    private readonly TestAppFactory _factory;

    public TasksPostTests(TestAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostTasks_CreatesTask_AndAppearsInGet()
    {
        var client = _factory.CreateClient();

        var postResponse = await client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest("Comprar leche"));
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(created);
        Assert.Equal("Comprar leche", created!.Description);
        Assert.False(created.Completed);

        var getResponse = await client.GetAsync("/api/tasks");
        var tasks = await getResponse.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.Contains(tasks!, t => t.Id == created.Id && t.Description == "Comprar leche");
    }
}
