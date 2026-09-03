using TodoApi.Models;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<TaskStore>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }))
    .WithName("GetHealth");

app.MapGet("/api/tasks", (TaskStore store) => Results.Ok(store.GetAll()))
    .WithName("GetTasks");

app.MapPost("/api/tasks", (CreateTaskRequest request, TaskStore store) =>
{
    var task = store.Add(request.Description);
    return Results.Created($"/api/tasks/{task.Id}", task);
})
    .WithName("CreateTask");

app.MapPut("/api/tasks/{id:int}", (int id, TaskStore store) =>
{
    var task = store.ToggleCompleted(id);
    return task is null ? Results.NotFound() : Results.Ok(task);
})
    .WithName("ToggleTask");

app.Run();

public partial class Program { }
