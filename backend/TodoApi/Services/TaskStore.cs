using TodoApi.Models;

namespace TodoApi.Services;

public class TaskStore
{
    private readonly List<TaskItem> _tasks = new();
    private int _nextId = 1;

    public IReadOnlyList<TaskItem> GetAll() => _tasks;

    public TaskItem Add(string description)
    {
        var task = new TaskItem { Id = _nextId++, Description = description, Completed = false };
        _tasks.Add(task);
        return task;
    }
}
