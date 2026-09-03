using System.Text.Json;
using TodoApi.Models;

namespace TodoApi.Services;

public class TaskStore
{
    private readonly string _filePath;
    private readonly object _lock = new();
    private readonly List<TaskItem> _tasks;
    private int _nextId;

    public TaskStore(IConfiguration configuration, IWebHostEnvironment env)
    {
        _filePath = configuration["TasksFilePath"]
            ?? Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "data", "tasks.json"));
        _tasks = Load();
        _nextId = _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;
    }

    private List<TaskItem> Load()
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
            return new List<TaskItem>();
        }

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(_tasks);
        File.WriteAllText(_filePath, json);
    }

    public IReadOnlyList<TaskItem> GetAll()
    {
        lock (_lock)
        {
            return _tasks.ToList();
        }
    }

    public TaskItem Add(string description)
    {
        lock (_lock)
        {
            var task = new TaskItem { Id = _nextId++, Description = description, Completed = false };
            _tasks.Add(task);
            Save();
            return task;
        }
    }

    public TaskItem? ToggleCompleted(int id)
    {
        lock (_lock)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task is null) return null;

            task.Completed = !task.Completed;
            Save();
            return task;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task is null) return false;

            _tasks.Remove(task);
            Save();
            return true;
        }
    }
}
