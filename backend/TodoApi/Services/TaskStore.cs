using TodoApi.Models;

namespace TodoApi.Services;

public class TaskStore
{
    private readonly List<TaskItem> _tasks = new();

    public IReadOnlyList<TaskItem> GetAll() => _tasks;
}
