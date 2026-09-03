using System.Net.Http.Json;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests;

public class TasksCarryOverTests : IClassFixture<TestAppFactory>
{
    private readonly TestAppFactory _factory;

    public TasksCarryOverTests(TestAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CarryOver_MovesPendingTask_ButNotCompletedTask()
    {
        var client = _factory.CreateClient();
        var week = new DateOnly(2026, 9, 21);
        var nextWeek = week.AddDays(7);

        var pendingResponse = await client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest("Pendiente de la semana", "media", week));
        var pending = await pendingResponse.Content.ReadFromJsonAsync<TaskItem>();

        var completedResponse = await client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest("Completada de la semana", "media", week));
        var completed = await completedResponse.Content.ReadFromJsonAsync<TaskItem>();
        await client.PutAsync($"/api/tasks/{completed!.Id}", content: null);

        var carryOverResponse = await client.PostAsync($"/api/tasks/carry-over?week={week:yyyy-MM-dd}", content: null);
        carryOverResponse.EnsureSuccessStatusCode();

        var tasks = await client.GetFromJsonAsync<List<TaskItem>>("/api/tasks") ?? new List<TaskItem>();
        var pendingAfter = tasks.Single(t => t.Id == pending!.Id);
        var completedAfter = tasks.Single(t => t.Id == completed.Id);

        Assert.Equal(nextWeek, pendingAfter.WeekStart);
        Assert.Equal(week, completedAfter.WeekStart);
    }
}
