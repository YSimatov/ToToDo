using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Authorization;
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
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    if (!string.IsNullOrEmpty(task.EndTime) && string.Compare(task.EndTime, task.StartTime) <= 0)
    {
        ModelState.AddModelError("EndTime", "End time must be after start time.");
        return BadRequest(ModelState);
    }

    _repository.Add(task);
    return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
  }

  [HttpPut("{id}")]
  public IActionResult Update(string id, TodoTask task)
  {
    if (id != task.Id)
    {
      return BadRequest("ID mismatch");
    }

    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    if (!string.IsNullOrEmpty(task.EndTime) && string.Compare(task.EndTime, task.StartTime) <= 0)
    {
        ModelState.AddModelError("EndTime", "End time must be after start time.");
        return BadRequest(ModelState);
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
  [Authorize(Roles = "Admin")]
  public IActionResult Delete(string id)
  {
    var task = _repository.GetById(id);
    if (task == null)
    {
      return NotFound();
    }

    _repository.Delete(id);
    return NoContent();
  }
}
