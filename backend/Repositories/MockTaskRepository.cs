using backend.Models;

namespace backend.Repositories;

public class MockTaskRepository : ITaskRepository
{
    private readonly List<TodoTask> _tasks;

    public MockTaskRepository()
    {
        _tasks = new List<TodoTask>
        {
            new TodoTask
            {
                Id = "1",
                Title = "Сделать дизайн проекта",
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                StartTime = "10:00",
                EndTime = "12:00",
                Completed = false,
                Description = "Закончить макет в Figma"
            },
            new TodoTask
            {
                Id = "2",
                Title = "Созвон с заказчиком",
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                StartTime = "14:00",
                EndTime = "15:00",
                Completed = true
            }
        };
    }

    public IEnumerable<TodoTask> GetAll()
    {
        return _tasks;
    }

    public TodoTask? GetById(string id)
    {
        return _tasks.FirstOrDefault(t => t.Id == id);
    }

    public void Add(TodoTask task)
    {
        if (string.IsNullOrEmpty(task.Id))
        {
            task.Id = Guid.NewGuid().ToString();
        }
        _tasks.Add(task);
    }

    public void Update(TodoTask task)
    {
        var existingTask = GetById(task.Id);
        if (existingTask != null)
        {
            existingTask.Title = task.Title;
            existingTask.Date = task.Date;
            existingTask.StartTime = task.StartTime;
            existingTask.EndTime = task.EndTime;
            existingTask.Completed = task.Completed;
            existingTask.Description = task.Description;
        }
    }

    public void Delete(string id)
    {
        var task = GetById(id);
        if (task != null)
        {
            _tasks.Remove(task);
        }
    }
}
