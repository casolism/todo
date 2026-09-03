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

    public TaskItem? ToggleCompleted(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task is null) return null;

        task.Completed = !task.Completed;
        return task;
    }
}
