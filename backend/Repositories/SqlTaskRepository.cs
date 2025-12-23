using backend.Models;
using Npgsql;

namespace backend.Repositories;

public class SqlTaskRepository : ITaskRepository
{
    private readonly string _connectionString;

    public SqlTaskRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

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

    public void Update(TodoTask task)
    {
        // For Lab 4, only one story is required in SQL.
        // I'll implement Update/Delete in SQL too for completeness, or leave for EF in Lab 5?
        // Lab 5 says "Implement remaining stories... at least one with EF".
        // So I can leave Update/Delete for EF.
        // But the interface requires implementation. I'll implement them in SQL for now to keep the app working,
        // and then in Lab 5 I can switch one of them to EF or add new features with EF.
        // Actually, Lab 5 says "Part on SQL, part on EF".
        // So let's keep GetAll/Add on SQL, and Update/Delete on EF in Lab 5.
        // But for now, I need a working app.
        // I will implement basic SQL for all now, and then REFACTOR Update/Delete to EF in Lab 5.
        
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("UPDATE \"Tasks\" SET \"Title\" = @Title, \"Date\" = @Date, \"StartTime\" = @StartTime, \"EndTime\" = @EndTime, \"Completed\" = @Completed, \"Description\" = @Description WHERE \"Id\" = @Id", conn))
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

    public void Delete(string id)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("DELETE FROM \"Tasks\" WHERE \"Id\" = @Id", conn))
            {
                cmd.Parameters.AddWithValue("Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
