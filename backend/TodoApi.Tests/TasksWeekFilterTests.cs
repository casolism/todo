using System.Net.Http.Json;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests;

public class TasksWeekFilterTests : IClassFixture<TestAppFactory>
{
    private readonly TestAppFactory _factory;

    public TasksWeekFilterTests(TestAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetTasks_FilteredByWeek_OnlyReturnsThatWeeksTasks()
    {
        var client = _factory.CreateClient();
        var week1 = new DateOnly(2026, 9, 7);
        var week2 = new DateOnly(2026, 9, 14);

        await client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest("Tarea de la semana 1", "media", week1));
        await client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest("Tarea de la semana 2", "media", week2));

        var response = await client.GetAsync($"/api/tasks?week={week1:yyyy-MM-dd}");
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskItem>>();

        Assert.Contains(tasks!, t => t.Description == "Tarea de la semana 1");
        Assert.DoesNotContain(tasks!, t => t.Description == "Tarea de la semana 2");
    }
}
