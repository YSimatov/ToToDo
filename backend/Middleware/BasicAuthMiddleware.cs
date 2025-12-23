using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Middleware;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;

    public BasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, AppDbContext dbContext)
    {
        // Allow OPTIONS requests for CORS preflight
        if (context.Request.Method == "OPTIONS")
        {
            await _next(context);
            return;
        }

        // Check if Authorization header is missing
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            // Allow access to public endpoints (like login/register if we had them separately, 
            // but for Basic Auth we often send creds with every request).
            // Lab says: "Передавать пароль между фронтендом и бэкендом в виде Basic-авторизации."
            // So we expect header on protected routes.
            // Let's protect everything except maybe initial load if needed?
            // Actually, let's protect /api/tasks.
            
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = 401;
                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"ToToDo\"";
                return;
            }
            
            await _next(context);
            return;
        }

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter!);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var username = credentials[0];
            var password = credentials[1];

            // Verify with DB
            // Password storage: Lab says "Пароль хранить в хешированном виде".
            // Implementation of hashing: Simple SHA256 or similar for demo.
            
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            
            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                // If user not found, maybe it's the first run or registration scenario via Basic Auth?
                // Lab says: "Вторая может быть 'незарегистрированный пользователь'... ему должен быть доступен какой-то ещё функционал кроме регистрации."
                // But for "Login", usually we check existing.
                // If we want to support "Auto-register" on first login for demo simplicity? No, that's bad.
                // We should probably have a registration endpoint or seed data.
                // I'll seed an admin user in Program.cs or just fail here.
                
                context.Response.StatusCode = 401;
                return;
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "Basic");
            var principal = new ClaimsPrincipal(identity);
            context.User = principal;

            await _next(context);
        }
        catch
        {
            context.Response.StatusCode = 401;
        }
    }

    private bool VerifyPassword(string password, string hash)
    {
        // Simple hash check (In real app use BCrypt/Argon2)
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = sha256.ComputeHash(bytes);
        var computedHash = Convert.ToBase64String(hashBytes);
        return computedHash == hash;
    }
}
