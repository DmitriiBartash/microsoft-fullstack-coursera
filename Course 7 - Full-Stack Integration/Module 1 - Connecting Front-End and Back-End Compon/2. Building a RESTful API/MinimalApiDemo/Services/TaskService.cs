using MinimalApiDemo.Models;

namespace MinimalApiDemo.Services;

public class TaskService
{
    private readonly List<TaskItem> _tasks =
    [
        new TaskItem(1, "Learn Minimal API", false),
        new TaskItem(2, "Test API with Scalar", true)
    ];

    public IEnumerable<TaskItem> GetAll() => _tasks;

    public TaskItem? GetById(int id) =>
        _tasks.FirstOrDefault(t => t.Id == id);

    public TaskItem Create(TaskItem item)
    {
        if (_tasks.Any(t => t.Id == item.Id))
            throw new InvalidOperationException($"Task with ID {item.Id} already exists.");

        _tasks.Add(item);
        return item;
    }

    public TaskItem? Update(int id, TaskItem updated)
    {
        var existing = _tasks.FirstOrDefault(t => t.Id == id);
        if (existing is null) return null;

        existing.Name = updated.Name;
        existing.IsCompleted = updated.IsCompleted;
        return existing;
    }

    public bool Delete(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task is null) return false;
        _tasks.Remove(task);
        return true;
    }
}
