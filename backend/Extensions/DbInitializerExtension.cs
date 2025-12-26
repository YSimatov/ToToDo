using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Extensions;

public static class DbInitializerExtension
{
    public static void InitializeDatabase(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
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
                        new User { Username = "admin", PasswordHash = adminPass, Role = "Admin" },
                        new User { Username = "user", PasswordHash = userPass, Role = "User" }
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
                throw;
            }
        }
    }
}