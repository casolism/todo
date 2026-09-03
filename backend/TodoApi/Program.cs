using TodoApi.Models;
using TodoApi.Services;

const string AngularDevCorsPolicy = "AngularDev";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<TaskStore>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularDevCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(AngularDevCorsPolicy);

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }))
    .WithName("GetHealth");

app.MapGet("/api/tasks", (string? week, TaskStore store) =>
{
    var tasks = store.GetAll();
    if (!string.IsNullOrEmpty(week) && DateOnly.TryParse(week, out var weekDate))
    {
        tasks = tasks.Where(t => t.WeekStart == weekDate).ToList();
    }
    return Results.Ok(tasks);
})
    .WithName("GetTasks");

app.MapPost("/api/tasks", (CreateTaskRequest request, TaskStore store) =>
{
    if (!TaskPriority.Valid.Contains(request.Priority))
    {
        return Results.BadRequest(new { error = $"Priority inválida. Valores permitidos: {string.Join(", ", TaskPriority.Valid)}" });
    }

    var weekStart = request.WeekStart ?? WeekUtils.GetWeekStart(DateOnly.FromDateTime(DateTime.Today));
    var task = store.Add(request.Description, request.Priority, weekStart);
    return Results.Created($"/api/tasks/{task.Id}", task);
})
    .WithName("CreateTask");

app.MapPut("/api/tasks/{id:int}", (int id, TaskStore store) =>
{
    var task = store.ToggleCompleted(id);
    return task is null ? Results.NotFound() : Results.Ok(task);
})
    .WithName("ToggleTask");

app.MapDelete("/api/tasks/{id:int}", (int id, TaskStore store) =>
    store.Delete(id) ? Results.NoContent() : Results.NotFound())
    .WithName("DeleteTask");

app.MapPost("/api/tasks/carry-over", (string week, TaskStore store) =>
{
    if (!DateOnly.TryParse(week, out var weekDate))
    {
        return Results.BadRequest(new { error = "El parámetro 'week' debe ser una fecha válida (YYYY-MM-DD)." });
    }

    var moved = store.CarryOverPending(weekDate);
    return Results.Ok(new { moved });
})
    .WithName("CarryOverTasks");

app.Run();

public partial class Program { }
