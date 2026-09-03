namespace TodoApi.Models;

public record CreateTaskRequest(string Description, string Priority = TaskPriority.Media, DateOnly? WeekStart = null);
