using backend.Data;
using backend.Repositories;
using Microsoft.EntityFrameworkCore;
using backend.Middleware;
using backend.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repository (Hybrid)
builder.Services.AddScoped<ITaskRepository, HybridTaskRepository>();

// Configure CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAngular",
      policy => policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

// Basic Auth Middleware
app.UseMiddleware<BasicAuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Ensure DB is created and Seed Data
using (var scope = app.Services.CreateScope())
{
  var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  try
  {
    context.Database.EnsureCreated();

    if (!context.Users.Any())
    {
      // Add Admin
      using var sha256 = System.Security.Cryptography.SHA256.Create();
      var adminPass = Convert.ToBase64String(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes("admin")));
      var userPass = Convert.ToBase64String(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes("user")));

      context.Users.AddRange(
          new backend.Models.User { Username = "admin", PasswordHash = adminPass, Role = "Admin" },
          new backend.Models.User { Username = "user", PasswordHash = userPass, Role = "User" }
      );
      context.SaveChanges();
    }
  }
  catch (Exception ex)
  {
    Console.WriteLine("--------------------------------------------------");
    Console.WriteLine("ERROR: Could not connect to PostgreSQL Database.");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine("Please check your connection string in 'appsettings.json'.");
    Console.WriteLine("Make sure PostgreSQL is running and credentials are correct.");
    Console.WriteLine("--------------------------------------------------");
    // Re-throw if you want to stop the app, or swallow to allow app to start (but API will fail)
    // Let's stop the app because it's useless without DB as per requirements.
    throw;
  }
}

app.Run();
