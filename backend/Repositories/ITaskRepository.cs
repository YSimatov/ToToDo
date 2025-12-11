using backend.Models;

namespace backend.Repositories;

public interface ITaskRepository
{
    IEnumerable<TodoTask> GetAll();
    TodoTask? GetById(string id);
    void Add(TodoTask task);
    void Update(TodoTask task);
    void Delete(string id);
}
