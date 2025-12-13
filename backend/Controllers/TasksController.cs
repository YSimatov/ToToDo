using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
  private readonly ITaskRepository _repository;

  public TasksController(ITaskRepository repository)
  {
    _repository = repository;
  }

  [HttpGet]
  public ActionResult<IEnumerable<TodoTask>> GetAll()
  {
    return Ok(_repository.GetAll());
  }

  [HttpGet("{id}")]
  public ActionResult<TodoTask> GetById(string id)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      return BadRequest("Id cannot be null or empty.");
    }
    var task = _repository.GetById(id);
    if (task == null)
    {
      return NotFound();
    }
    return Ok(task);
  }

  [HttpPost]
  public ActionResult<TodoTask> Create(TodoTask task)
  {
    if (task == null)
    {
      return BadRequest("Task cannot be null.");
    }
    _repository.Add(task);
    return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
  }

  [HttpPut("{id}")]
  public IActionResult Update(string id, TodoTask task)
  {
    if (string.IsNullOrWhiteSpace(id) || task == null)
    {
       return BadRequest("Invalid input data.");
    }
    if (id != task.Id)
    {
      return BadRequest("Id mismatch.");
    }

    var existingTask = _repository.GetById(id);
    if (existingTask == null)
    {
      return NotFound();
    }

    _repository.Update(task);
    return NoContent();
  }

  [HttpDelete("{id}")]
  public IActionResult Delete(string id)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
       return BadRequest("Id cannot be null or empty.");
    }
    var task = _repository.GetById(id);
    if (task == null)
    {
      return NotFound();
    }

    _repository.Delete(id);
    return NoContent();
  }
}
