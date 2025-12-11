using backend.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register Repository (Mock for Lab 3)
builder.Services.AddSingleton<ITaskRepository, MockTaskRepository>();

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

// app.UseAuthorization(); // Not needed for Lab 3 but harmless

app.MapControllers();

app.Run();
