using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace backend.Repositories;

public class HybridTaskRepository : ITaskRepository
{
    private readonly string _connectionString;
    private readonly AppDbContext _context;

    public HybridTaskRepository(IConfiguration configuration, AppDbContext context)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        _context = context;
    }

    // Implemented via SQL (Lab 4 / Lab 5 Part 1)
    public IEnumerable<TodoTask> GetAll()
    {
        var tasks = new List<TodoTask>();
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("SELECT \"Id\", \"Title\", \"Date\", \"StartTime\", \"EndTime\", \"Completed\", \"Description\" FROM \"Tasks\"", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tasks.Add(new TodoTask
                    {
                        Id = reader.GetString(0),
                        Title = reader.GetString(1),
                        Date = reader.GetString(2),
                        StartTime = reader.GetString(3),
                        EndTime = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Completed = reader.GetBoolean(5),
                        Description = reader.IsDBNull(6) ? null : reader.GetString(6)
                    });
                }
            }
        }
        return tasks;
    }

    // Implemented via SQL
    public TodoTask? GetById(string id)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("SELECT \"Id\", \"Title\", \"Date\", \"StartTime\", \"EndTime\", \"Completed\", \"Description\" FROM \"Tasks\" WHERE \"Id\" = @Id", conn))
            {
                cmd.Parameters.AddWithValue("Id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new TodoTask
                        {
                            Id = reader.GetString(0),
                            Title = reader.GetString(1),
                            Date = reader.GetString(2),
                            StartTime = reader.GetString(3),
                            EndTime = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Completed = reader.GetBoolean(5),
                            Description = reader.IsDBNull(6) ? null : reader.GetString(6)
                        };
                    }
                }
            }
        }
        return null;
    }

    // Implemented via SQL
    public void Add(TodoTask task)
    {
        if (string.IsNullOrEmpty(task.Id))
        {
            task.Id = Guid.NewGuid().ToString();
        }

        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("INSERT INTO \"Tasks\" (\"Id\", \"Title\", \"Date\", \"StartTime\", \"EndTime\", \"Completed\", \"Description\") VALUES (@Id, @Title, @Date, @StartTime, @EndTime, @Completed, @Description)", conn))
            {
                cmd.Parameters.AddWithValue("Id", task.Id);
                cmd.Parameters.AddWithValue("Title", task.Title);
                cmd.Parameters.AddWithValue("Date", task.Date);
                cmd.Parameters.AddWithValue("StartTime", task.StartTime);
                cmd.Parameters.AddWithValue("EndTime", (object?)task.EndTime ?? DBNull.Value);
                cmd.Parameters.AddWithValue("Completed", task.Completed);
                cmd.Parameters.AddWithValue("Description", (object?)task.Description ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // Implemented via Entity Framework (Lab 5 Part 2)
    public void Update(TodoTask task)
    {
        // Attach if not tracked, or just update
        // Since we are in a scoped request, _context is fresh.
        // We can just set the state to Modified.
        _context.Tasks.Update(task);
        _context.SaveChanges();
    }

    // Implemented via Entity Framework (Lab 5 Part 2)
    public void Delete(string id)
    {
        var task = _context.Tasks.Find(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            _context.SaveChanges();
        }
    }
}
