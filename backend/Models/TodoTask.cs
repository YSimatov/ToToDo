namespace backend.Models;

public class TodoTask
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty; // YYYY-MM-DD
    public string StartTime { get; set; } = string.Empty; // HH:mm
    public string? EndTime { get; set; }
    public bool Completed { get; set; }
    public string? Description { get; set; }
}
